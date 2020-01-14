using System.ComponentModel.DataAnnotations.Schema;

namespace MusicShare.Models
{
    public class Song
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public Artist Artist { get; set; }

        public int GenreId { get; set; }

        [ForeignKey("GenreId")]
        public Genre Genre { get; set; }

    }
}
