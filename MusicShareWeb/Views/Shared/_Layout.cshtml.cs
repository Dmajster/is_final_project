using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShare.Data;
using MusicShare.Models;

namespace MusicShareWeb.Views.Shared
{
    [AllowAnonymous]
    public class HomeIndexModel : PageModel
    {
        private readonly MusicShareContext _context;

        public HomeIndexModel(MusicShareContext context)
        {
            _context = context;
        }

        public IList<Post> Post { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public SelectList Posts { get; set; }

        public async Task OnGetAsync()
        {
            var posts = _context.Posts.ToList();

            if (!string.IsNullOrEmpty(SearchString))
            {
                posts = posts
                    .Where(post => post.Song.Name.Contains(SearchString))
                    .ToList();
            }

            Posts = new SelectList(posts);
        }
    }
}
