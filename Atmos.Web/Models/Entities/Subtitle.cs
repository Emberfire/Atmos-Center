using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Atmos.Web.Models.Entities
{
    public class Subtitle
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Language { get; set; }
        [Required]
        public string Path { get; set; }

        [Required]
        public virtual Movie Movie { get; set; }
    }
}
