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

        public FileInfo GetSingleFileOfType(DirectoryInfo directoryInfo, FileType fileType)
        {
            string searchPattern = fileType switch
            {
                FileType.Mp4 => "*.mp4",
                FileType.Mkv => "*.mkv",
                FileType.Avi => "*.avi",
                FileType.Vtt => "*.vtt",
                FileType.Srt => "*.srt",
                _ => throw new ArgumentException("Provided file type is invalid!", nameof(fileType)),
            };
            return directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories).ToList().First();
        }
        public List<FileInfo> GetFilesOfType(string path, FileType fileType)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Provided folder path is invalid!", nameof(path));
            }

            string searchPattern = fileType switch
            {
                FileType.Mp4 => "*.mp4",
                FileType.Mkv => "*.mkv",
                FileType.Avi => "*.avi",
                FileType.Vtt => "*.vtt",
                FileType.Srt => "*.srt",
                _ => throw new ArgumentException("Provided file type is invalid!", nameof(fileType)),
            };

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            return directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories).ToList();
        }
        public List<FileInfo> GetFilesOfType(string path, IEnumerable<FileType> fileTypes)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Provided folder path is invalid!", nameof(path));
            }

            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            foreach (var fileType in fileTypes)
            {
                string searchPattern = fileType switch
                {
                    FileType.Mp4 => "*.mp4",
                    FileType.Mkv => "*.mkv",
                    FileType.Avi => "*.avi",
                    FileType.Vtt => "*.vtt",
                    FileType.Srt => "*.srt",
                    _ => throw new ArgumentException("Provided file type is invalid!", nameof(fileTypes)),
                };

                files.AddRange(directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories));
            }

            return files;
        }
        public void RemoveFilesByPattern(string pattern, List<FileInfo> files)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException("Provided pattern is empty!", nameof(pattern));
            }

            if (files is null)
            {
                throw new ArgumentNullException("Provided array of files is invalid!", nameof(files));
            }

            Regex regex = new Regex(pattern);

            files.RemoveAll(file =>
            {
                Match match = regex.Match(file.Name.Split(".")[0]);
                return match.Success;
            });
        }

        public void RescanDirectoryForMovies(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Provided folder path is empty!", nameof(path));
            }

            PurgeMovies();

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            List<FileInfo> files = GetFilesOfType(path, new[] { FileType.Mp4, FileType.Mkv, FileType.Avi });

            RemoveFilesByPattern(Consts.EpisodeRegexPattern, files);

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
                //if (Context.Movies.Any(m => m.Title == movie.Title))
                //{
                //    Context.UpdateMovie(movie);
                //}
                //else
                //{
                    Context.Movies.Add(movie);
                //}
            }

            Context.SaveChanges();
            RescanDirectoryForSubtitles(path);
        }
        public void RescanDirectoryForSubtitles(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Provided folder path is empty!", nameof(path));
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            List<FileInfo> files = GetFilesOfType(path, new[] { FileType.Srt, FileType.Vtt });

            RemoveFilesByPattern(Consts.EpisodeRegexPattern, files);

            List<Subtitle> subtitles = files.Select((file, index) =>
            {
                Subtitle subtitle = new Subtitle()
                {
                    Language = "en",
                    Path = file.FullName
                };

                string title = file.Name.Split(".")[0];
                Movie movie = Context.Movies.FirstOrDefault(m => m.Title == title);
                if (movie is null)
                {
                    // If the subtitle has a different title than the movie,
                    //scan its directory for movies and use the first available one.
                    List<FileInfo> movieWithDifferentName = GetFilesOfType(file.Directory.FullName, new[] { FileType.Mp4, FileType.Mkv, FileType.Avi });

                    title = movieWithDifferentName[0].Name.Split(".")[0];
                    movie = Context.Movies.FirstOrDefault(m => m.Title == title);
                }

                if (movie != null)
                {
                    subtitle.Movie = movie;
                    //if (Context.Subtitles.Any(s => s.Path == subtitle.Path))
                    //{
                    //    Context.UpdateSubtitle(subtitle);
                    //}
                    //else
                    //{
                    movie.Subtitles.Add(subtitle);
                    //}
                }
                else
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
        public void PurgeMovies()
        {
            List<Movie> movies = Context.Movies.ToList();
            Context.Movies.RemoveRange(movies);

            Context.SaveChanges();
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            List<Movie> movies = await Context.Movies.OrderBy(movie => movie.Title).ToListAsync().ConfigureAwait(false);
            return movies;
        }
        public async Task<List<Movie>> GetMoviesByTypeAsync(string type)
        {
            List<Movie> movies = await Context.Movies.Where(m => m.Extension == type).OrderBy(movie => movie.Title).ToListAsync().ConfigureAwait(false);
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
