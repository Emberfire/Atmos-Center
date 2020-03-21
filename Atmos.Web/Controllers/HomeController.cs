using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Atmos.Web.Models.ViewModels;
using System.IO;
using System.Text.RegularExpressions;
using Atmos.Web.Logic.Client;
using Atmos.Web.Models;
using Atmos.Web.Data;

namespace Atmos.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAtmosClientSession Session;
        public HomeController(ILogger<HomeController> logger, AtmosContext context)
        {
            _logger = logger;
            Session = new AtmosClientSession(context);
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

        public async Task<IActionResult> Movies()
        {
            object b = TempData["trusted"];
            if (b != null)
            {
                List<Models.Entities.Movie> movies = await Session.GetAllMoviesAsync();
                IEnumerable<MovieViewModel> model = movies.Select((movie, index) =>
                {
                    return new MovieViewModel()
                    {
                        Id = movie.Id,
                        Title = movie.Title,
                        Extension = movie.Extension
                    };
                });
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Watch(string id)
        {
            var movie = await Session.GetMovieAsync(id);
            //(new NReco.VideoConverter.FFMpegConverter()).ConvertMedia(pathToVideoFile, pathToOutputMp4File, Formats.mp4)
            var model = new MovieViewModel()
            {
                Id = movie.Id,
                Title = movie.Title,
                Extension = movie.Extension
            };

            foreach (var subtitle in movie.Subtitles)
            {
                model.Subtitles.Add(subtitle.Language, subtitle.Id);
            }
            //if (path.Split(".")[1] == "mkv")
            //{
            //    var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            //    var newPath = Uri.UnescapeDataString(path).Split(".")[0] + ".mp4";
            //    ffMpeg.ConvertMedia(path, newPath, Format.mp4);
            //}

            //ViewBag.Subs = path.Split(".")[0] + ".vtt";
            return View(model);
        }

        public async Task<IActionResult> GetVideo(string id)
        {
            Models.Entities.Movie movie = await Session.GetMovieAsync(id);
            if (movie != null)
            {
                return PhysicalFile(movie.Path, "application/octet-stream", enableRangeProcessing: true);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> GetSubtitle(string id)
        {
            Models.Entities.Subtitle subtitles = await Session.GetSubtitleAsync(Uri.UnescapeDataString(id));
            if (subtitles != null)
            {
                return PhysicalFile(subtitles.Path, "text/vtt");
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Rescan(string path)
        {
            Session.ScanFolderForMovies(Uri.UnescapeDataString(path), SearchOption.AllDirectories);
            Session.ScanFolderForSubtitles(Uri.UnescapeDataString(path), SearchOption.AllDirectories);
            return Content("Success");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
