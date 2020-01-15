using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MusicShare.Data;
using MusicShare.Models;

namespace MusicShareWeb.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PostsController : Controller
    {
        private readonly MusicShareContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _webHostEnvironment;
        public PostsController(MusicShareContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHostEnvironment = environment;
            _userManager = userManager;
        }

        private IEnumerable<SelectListItem> GetSongs()
        {
            return _context.Songs
                .Select(song => new SelectListItem
                {
                    Value = song.Id.ToString(),
                    Text = song.Name
                })
                .ToList();
        }
        private IEnumerable<SelectListItem> GetArtists()
        {
            return _context.Artists
                .Select(artist => new SelectListItem
                {
                    Value = artist.Id.ToString(),
                    Text = artist.Name
                })
                .ToList();
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var musicShareContext = _context.Posts.Include(p => p.Artist).Include(p => p.Song);
            return View(await musicShareContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Artist)
                .Include(p => p.Song)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewBag.SongList = new SelectList(GetSongs(), "Value", "Text");
            ViewBag.ArtistsList = new SelectList(GetArtists(), "Value", "Text");
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Show(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Artist)
                .Include(p => p.Song)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            post.ViewCount++;
            await _context.SaveChangesAsync();

            return View(post);
        }


        public class InsertPostModel
        {
            public int Id { get; set; }

            [Required]
            public int SongId { get; set; }

            [ForeignKey("SongId")]
            public Song Song { get; set; }

            [Required]
            public int ArtistId { get; set; }

            [ForeignKey("ArtistId")]
            public Artist Artist { get; set; }

            [Required]
            public string YoutubeLink { get; set; }

            public string PdfFilePath { get; set; }
        }


        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SongId,ArtistId,ViewCount,YoutubeLink,PdfFilePath,Reviewed")] InsertPostModel insertPost, List<IFormFile> files)
        {
            var isNormalYoutubeLink = Regex.IsMatch(insertPost.YoutubeLink, "((http(s)?://)(www.)?)?youtube.com\\/watch\\?v=([^#&?]).*");

            if (ModelState.IsValid && isNormalYoutubeLink)
            {
                var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                var post = new Post
                {
                    ArtistId = insertPost.ArtistId,
                    SongId = insertPost.SongId,
                    Reviewed = false,
                    ViewCount = 0
                };

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var directory = Path.GetDirectoryName(uploads);

                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        var fileName = Path.GetRandomFileName();
                        fileName = fileName.Replace(".", "a");
                        fileName += ".pdf";

                        var fullPath = Path.Combine(uploads, fileName);

                        post.PdfFilePath = fileName;

                        // If file with same name exists delete it
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                        // Create new local file and copy contents of uploaded file
                        using (var localFile = System.IO.File.OpenWrite(fullPath))
                        using (var uploadedFile = file.OpenReadStream())
                        {
                            uploadedFile.CopyTo(localFile);
                        }
                    }
                }

                var videoId = insertPost.YoutubeLink.Split("=")[1];

                post.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                post.YoutubeLink = $"https://www.youtube.com/embed/{videoId}";
                post.ThumbnailLink = $"https://img.youtube.com/vi/{videoId}/0.jpg";

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id", insertPost.ArtistId);
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", insertPost.SongId);

            return View(insertPost);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewBag.SongList = new SelectList(GetSongs(), "Value", "Text");
            ViewBag.ArtistsList = new SelectList(GetArtists(), "Value", "Text");
            return View(post);
        }
        public class EditPostModel
        {
            public int Id { get; set; }

            [Required]
            public int SongId { get; set; }

            [ForeignKey("SongId")]
            public Song Song { get; set; }

            [Required]
            public int ArtistId { get; set; }

            [ForeignKey("ArtistId")]
            public Artist Artist { get; set; }

            [Required]
            public string YoutubeLink { get; set; }

            [Required]
            public string PdfFilePath { get; set; }

            [Required]
            public int ViewCount { get; set; }

            [Required]
            public bool Reviewed { get; set; }
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SongId,ArtistId,ViewCount,YoutubeLink,PdfFilePath,Reviewed")] EditPostModel editPost)
        {
            if (id != editPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var post = _context.Find<Post>(editPost.Id);
                    post.Id = editPost.Id;
                    post.ArtistId = editPost.ArtistId;
                    post.SongId = editPost.SongId;
                    post.ViewCount = editPost.ViewCount;
                    post.YoutubeLink = editPost.YoutubeLink;
                    post.PdfFilePath = editPost.PdfFilePath;
                    post.Reviewed = editPost.Reviewed;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(editPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id", editPost.ArtistId);
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", editPost.SongId);
            return View(editPost);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Artist)
                .Include(p => p.Song)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        public IActionResult ChordCheck(int button)
        {
            if (button == 0)
            {
                TempData["chord"] = "Chord C selected";
            }
            else
            {
                TempData["chord"] = "Chord 213elected";
            }
            return RedirectToAction("Index");
        }


    }
}
