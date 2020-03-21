using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static Atmos.Web.Logic.Constants.CommonTypes;

namespace Atmos.Web.Models.Entities
{
    public class Movie
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Title { get; set; }

        [Required]
        public string Path { get; set; }
        public string SubtitlePath { get; set; }
        [Required]
        public string Extension { get; set; }
    }
}
