using System.ComponentModel.DataAnnotations;

namespace Atmos.Web.Data.Entities
{
    public class Subtitle
    {
        public string Id { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public string Path { get; set; }

        [Required]
        public virtual Movie Movie { get; set; }
    }
}
