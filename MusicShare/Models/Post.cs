using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace MusicShare.Models
{
    public class Post
    {
#nullable enable
        public int Id { get; set; }

        public int? SongId { get; set; }

        [ForeignKey("SongId")]
        public Song? Song { get; set; }

        public int? ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public Artist? Artist { get; set; }

        public int ViewCount { get; set; }

        public string YoutubeLink { get; set; }

        public string PdfFilePath { get; set; }

        public bool Reviewed { get; set; }

        public IFormFile PdfFile { get; set; }

        public ApplicationUser? Owner { get; set; }
#nullable disable

    }
}
