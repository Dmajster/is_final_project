using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MusicShare.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Favorite> Favorites { get; set; }
    }
}
