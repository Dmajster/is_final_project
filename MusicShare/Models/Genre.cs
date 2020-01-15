using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicShare.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Song> Songs { get; set; }
    }
}
