using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MusicShare.Data;
using MusicShare.Models;

namespace MusicShareWeb.Controllers
{
    public class PostsController : Controller
    {
        private readonly MusicShareContext _context;
        private IWebHostEnvironment _webHostEnvironment;
        public PostsController(MusicShareContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _webHostEnvironment = environment;
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

        public IActionResult Show()
        {
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id");
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id");
            return View();
        }


        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SongId,ArtistId,ViewCount,YoutubeLink,PdfFilePath,Reviewed")] Post post, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

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

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id", post.ArtistId);
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", post.SongId);

            return View(post);
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

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SongId,ArtistId,ViewCount,YoutubeLink,PdfFilePath,Reviewed")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id", post.ArtistId);
            ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Id", post.SongId);
            return View(post);
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
    }
}
