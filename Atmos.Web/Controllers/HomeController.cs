using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Atmos.Web.Models;
using System.IO;

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
            password = Uri.UnescapeDataString(password);
            if (password == "c3ntripetal")
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
            bool a = (bool)TempData["trusted"];
            if (a == true)
            {
                var movies = new Dictionary<string, string>();
                DirectoryInfo di = new DirectoryInfo(@"D:\Movies");
                FileInfo[] files = di.GetFiles("*.mp4", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    movies.Add(files[i].Name.Split(".")[0], Uri.EscapeDataString(files[i].FullName));
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
