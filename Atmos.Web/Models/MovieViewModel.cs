using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmos.Web.Models
{
    public class MovieViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }

        public string Cover { get; set; }
        public Dictionary<string, string> Subtitles { get; set; } = new Dictionary<string, string>();
    }
}