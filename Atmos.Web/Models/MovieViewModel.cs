using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Atmos.Web.Logic.Constants.CommonTypes;

namespace Atmos.Web.Models
{
    public class MovieViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
    }
}