using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Atmos.Web.Models;
using System.IO;
using System.Text.RegularExpressions;

namespace Atmos.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string password)
        {
            if (!string.IsNullOrEmpty(password) && Uri.UnescapeDataString(password) == "c3ntripetal")
            {
                TempData["trusted"] = true;
                return RedirectToAction("Movies");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Movies()
        {
            object b = TempData["trusted"];
            if (b != null)
            {
                var movies = new Dictionary<string, string>();
                DirectoryInfo di = new DirectoryInfo(@"D:\Movies");
                FileInfo[] files = di.GetFiles("*.mp4", SearchOption.AllDirectories);
                FileInfo[] mkvFiles = di.GetFiles("*.mkv", SearchOption.AllDirectories);

                Regex regex = new Regex("^Episode [0-9]+ ?•?");
                for (int i = 0; i < files.Length; i++)
                {
                    Match match = regex.Match(files[i].Name.Split(".")[0]);

                    if (!match.Success)
                    {
                        movies.Add(files[i].Name.Split(".")[0], Uri.EscapeDataString(files[i].FullName));
                    }
                }
                for (int i = 0; i < mkvFiles.Length; i++)
                {
                    Match match = regex.Match(mkvFiles[i].Name.Split(".")[0]);

                    if (!match.Success)
                    {
                        movies.Add(mkvFiles[i].Name.Split(".")[0], Uri.EscapeDataString(mkvFiles[i].FullName));
                    }
                }

                return View(movies);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Watch(string path)
        {
            ViewBag.Id = path;
            return View();
        }

        public IActionResult GetVideo(string path)
        {
            path = Uri.UnescapeDataString(path);
            return PhysicalFile(path, "application/octet-stream", enableRangeProcessing: true);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
