using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atmos.Web.Data.Entities
{
    public class Movie
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Extension { get; set; }

        public virtual ICollection<Subtitle> Subtitles { get; }
    }
}
