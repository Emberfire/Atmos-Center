using Atmos.Web.Data;
using Atmos.Web.Logic.Constants;
using Atmos.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Atmos.Web.Logic.Constants.CommonTypes;

namespace Atmos.Web.Logic.Client
{
    class AtmosClientSession : IAtmosClientSession
    {
        public string SessionId { get; set; }
        private readonly AtmosContext Context;

        public AtmosClientSession(AtmosContext context)
        {
            Context = context;
        }

        List<FileInfo> GetFilesOfType(DirectoryInfo directoryInfo, FileType fileType, SearchOption searchOption)
        {
            string searchPattern = fileType switch
            {
                FileType.Mp4 => "*.mp4",
                FileType.Mkv => "*.mkv",
                FileType.Avi => "*.avi",
                FileType.Vtt => "*.vtt",
                _ => throw new ArgumentException(nameof(fileType)),
            };
            return directoryInfo.EnumerateFiles(searchPattern, searchOption).ToList();
        }
        void RemoveFilesByPattern(Regex regex, List<FileInfo> files)
        {
            if (regex is null)
            {
                throw new ArgumentNullException(nameof(regex));
            }

            if (files is null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            files.RemoveAll(file =>
            {
                Match match = regex.Match(file.Name.Split(".")[0]);
                return match.Success;
            });
        }
        public void ScanFolderForMovies(string folderPath, SearchOption searchOption)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentException(nameof(folderPath));
            }


            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            List<FileInfo> files = new List<FileInfo>();
            files.AddRange(GetFilesOfType(directoryInfo, FileType.Mp4, searchOption));
            files.AddRange(GetFilesOfType(directoryInfo, FileType.Mkv, searchOption));
            files.AddRange(GetFilesOfType(directoryInfo, FileType.Avi, searchOption));

            Regex regex = new Regex("^Episode [0-9]+ ?•?");
            RemoveFilesByPattern(regex, files);

            List<Movie> movies = files.Select((file, index) =>
            {
                return new Movie()
                {
                    Path = file.FullName,
                    Title = file.Name.Split(".")[0],
                    Extension = file.Extension
                };
            }).OrderBy(movie => movie.Title).ToList();

            foreach (Movie movie in movies)
            {
                bool alreadyExists = Context.Movies.Any(m => m.Title == movie.Title);
                if (!alreadyExists)
                {
                    Context.Movies.Add(movie);
                }
            }
            
            Context.SaveChanges();
        }
        public void ScanFolderForSubtitles(string folderPath, SearchOption searchOption)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentException(nameof(folderPath));
            }


            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            List<FileInfo> files = new List<FileInfo>();
            files.AddRange(GetFilesOfType(directoryInfo, FileType.Vtt, searchOption));

            Regex regex = new Regex("^Episode [0-9]+ ?•?");
            RemoveFilesByPattern(regex, files);

            List<Subtitle> subtitles = files.Select((file, index) =>
            {
                Subtitle subtitle = new Subtitle()
                {
                    Language = "en",
                    Path = file.FullName
                };

                string title = file.Name.Split(".")[0];
                Movie movie = Context.Movies.FirstOrDefault(m => m.Title == title);
                if (movie != null)
                {
                    subtitle.Movie = movie;
                    movie.Subtitles.Add(subtitle);
                } else
                {
                    throw new Exception("Subtitle doesn't have a movie!");
                }

                return subtitle;
            }).OrderBy(subtitle => subtitle.Movie.Title).ToList();

            foreach (Subtitle subtitle in subtitles)
            {
                bool alreadyExists = Context.Subtitles.Any(s => s.Movie.Title == subtitle.Movie.Title);
                if (!alreadyExists)
                {
                    Context.Subtitles.Add(subtitle);
                }
            }

            Context.SaveChanges();
        }
        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            List<Movie> movies = await Context.Movies.OrderBy(movie => movie.Title).ToListAsync();
            return movies;
        }
        public async Task<Movie> GetMovieAsync(string id)
        {
            Movie movie = await Context.Movies.FindAsync(id);
            return movie;
        }
        public async Task<Subtitle> GetSubtitleAsync(string id)
        {
            Subtitle subtitle = await Context.Subtitles.FindAsync(id);
            return subtitle;
        }
        public async Task<List<Subtitle>> GetMovieSubtitlesAsync(string id)
        {
            Movie movie = await Context.Movies.FindAsync(id);
            return movie.Subtitles;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
