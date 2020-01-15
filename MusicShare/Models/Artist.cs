using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicShare.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Song> Songs { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
