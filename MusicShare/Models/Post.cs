using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicShare.Models
{
    public class Post
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
        public int ViewCount { get; set; }

        [Required]
        public string YoutubeLink { get; set; }

        [Required]
        public string ThumbnailLink { get; set; }

        [Required]
        public string PdfFilePath { get; set; }

        [Required]
        public bool Reviewed { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
