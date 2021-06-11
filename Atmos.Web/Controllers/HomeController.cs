using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Atmos.Web.Logic.Client;
using Atmos.Web.Models;
using Atmos.Web.Data;
using Atmos.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Atmos.Web.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IAtmosClientSession Session;
        public HomeController(/*ILogger<HomeController> logger, */AtmosContext context)
        {
            //_logger = logger;
            Session = new AtmosClientSession(context);
        }

        public IActionResult Index()
        {
            if (Request.Cookies["trusted"] == "true")
            {
                return RedirectToAction("Movies");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Index(string password)
        {
            if (!string.IsNullOrEmpty(password) && Uri.UnescapeDataString(password) == "c3ntripetal")
            {
                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1)
                };

                Response.Cookies.Append("trusted", "true", options);
                return RedirectToAction("Movies");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Movies()
        {
            if (Request.Cookies["trusted"] != "true")
            {
                return RedirectToAction("Index");
            }

            IEnumerable<Movie> movies = await Session.GetAllMoviesAsync().ConfigureAwait(false);
            IEnumerable<MovieViewModel> model = movies.Select(movie =>
            {
                MovieViewModel viewModel = new()
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Extension = movie.Extension
                };

                return viewModel;
            });
            
            return View(model);

        }

        public ActionResult GetCover(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            Movie movie = Session.GetMovie(id);
            try
            {
                string coverPath = Path.Combine(new DirectoryInfo(movie.Path).Parent.FullName, "cover.jpg");
                FileStream image = System.IO.File.OpenRead(coverPath);
                return File(image, "image/jpg");
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Watch(string id)
        {
            Movie movie = await Session.GetMovieAsync(id).ConfigureAwait(false);
            MovieViewModel model = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Extension = movie.Extension
            };

            foreach (Subtitle subtitle in movie.Subtitles)
            {
                model.Subtitles.Add(subtitle.Language, subtitle.Id);
            }

            return View(model);
        }

        public async Task<IActionResult> GetVideo(string id)
        {
            Movie movie = await Session.GetMovieAsync(id).ConfigureAwait(false);
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
            Subtitle subtitles = await Session.GetSubtitleAsync(Uri.UnescapeDataString(id)).ConfigureAwait(false);
            if (subtitles != null)
            {
                return PhysicalFile(subtitles.Path, "application/json");
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Rescan()
        {
            Session.RescanDirectoryForMovies(@"W:\Movies");
            return Content("Success");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
