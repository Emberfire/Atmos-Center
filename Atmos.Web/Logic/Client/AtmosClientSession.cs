using Atmos.Web.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Atmos.Web.Data.Entities;
using Atmos.Web.Resources;
using static Atmos.Web.Logic.Constants.CommonTypes;

namespace Atmos.Web.Logic.Client
{
    class AtmosClientSession : IAtmosClientSession
    {
        public string SessionId { get; set; }
        private readonly AtmosContext _context;

        public AtmosClientSession(AtmosContext context)
        {
            _context = context;
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
                FileType.Sub => "*.sub",
                _ => throw new ArgumentException(en.InvalidFileType, nameof(fileType)),
            };
            return directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories).ToList().First();
        }
        public IEnumerable<FileInfo> GetFilesOfType(string path, FileType fileType)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(en.InvalidFolderPath, nameof(path));
            }

            string searchPattern = fileType switch
            {
                FileType.Mp4 => "*.mp4",
                FileType.Mkv => "*.mkv",
                FileType.Avi => "*.avi",
                FileType.Vtt => "*.vtt",
                FileType.Srt => "*.srt",
                FileType.Sub => "*.sub",
                _ => throw new ArgumentException(en.InvalidFileType, nameof(fileType)),
            };

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            return directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories).ToList();
        }
        public IEnumerable<FileInfo> GetFilesOfType(string path, IEnumerable<FileType> fileTypes)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(en.InvalidFolderPath, nameof(path));
            }

            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            foreach (FileType fileType in fileTypes)
            {
                string searchPattern = fileType switch
                {
                    FileType.Mp4 => "*.mp4",
                    FileType.Mkv => "*.mkv",
                    FileType.Avi => "*.avi",
                    FileType.Vtt => "*.vtt",
                    FileType.Srt => "*.srt",
                    FileType.Sub => "*.sub",
                    _ => throw new ArgumentException(en.InvalidFileType, nameof(fileTypes)),
                };

                files.AddRange(directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories));
            }

            return files;
        }
        public void RemoveFilesByPattern(string pattern, IEnumerable<FileInfo> files)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException(en.EmptyPattern, nameof(pattern));
            }

            if (files is null)
            {
                throw new ArgumentNullException(nameof(files), en.InvalidFileCollection);
            }

            Regex regex = new Regex(pattern);

            files.ToList().RemoveAll(file =>
            {
                Match match = regex.Match(file.Name.Split(".")[0]);
                return match.Success;
            });
        }

        public void RescanDirectoryForMovies(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(en.EmptyFolderPath, nameof(path));
            }

            PurgeMovies();

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            IEnumerable<FileInfo> files = GetFilesOfType(path, new[] { FileType.Mp4, FileType.Mkv, FileType.Avi });

            //RemoveFilesByPattern(Constants.Constants.EpisodeRegexPattern, files);

            List<Movie> movies = files.Select((file, index) => new Movie
            {
                Id = Guid.NewGuid().ToString(),
                Path = file.FullName,
                Title = file.Name.Split(".")[0],
                Extension = file.Extension
            }).OrderBy(movie => movie.Title).ToList();

            foreach (Movie movie in movies)
            {
                //if (Context.Movies.Any(m => m.Title == movie.Title))
                //{
                //    Context.UpdateMovie(movie);
                //}
                //else
                //{
                    _context.Movies.Add(movie);
                //}
            }

            _context.SaveChanges();
            //RescanDirectoryForSubtitles(path);
        }
        public void RescanDirectoryForSubtitles(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(en.EmptyFolderPath, nameof(path));
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            IEnumerable<FileInfo> files = GetFilesOfType(path, new[] { FileType.Srt, FileType.Vtt });

            //RemoveFilesByPattern(Constants.Constants.EpisodeRegexPattern, files);

            List<Subtitle> subtitles = files.Select((file, index) =>
            {
                Subtitle subtitle = new Subtitle()
                {
                    Language = "en",
                    Path = file.FullName
                };

                string title = file.Name.Split(".")[0];
                Movie movie = _context.Movies.FirstOrDefault(m => m.Title == title);
                if (movie is null)
                {
                    // If the subtitle has a different title than the movie,
                    //scan its directory for movies and use the first available one.
                    IEnumerable<FileInfo> movieWithDifferentName = GetFilesOfType(file.Directory?.FullName, new[] { FileType.Mp4, FileType.Mkv, FileType.Avi });

                    title = movieWithDifferentName.ToList()[0].Name.Split(".")[0];
                    movie = _context.Movies.FirstOrDefault(m => m.Title == title);
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
                bool alreadyExists = _context.Subtitles.Any(s => s.Movie.Title == subtitle.Movie.Title);
                if (!alreadyExists)
                {
                    _context.Subtitles.Add(subtitle);
                }
            }

            _context.SaveChanges();
        }
        public void PurgeMovies()
        {
            List<Movie> movies = _context.Movies.ToList();
            _context.Movies.RemoveRange(movies);

            _context.SaveChanges();
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            List<Movie> movies = await _context.Movies.OrderBy(movie => movie.Title).ToListAsync().ConfigureAwait(false);
            return movies;
        }
        public async Task<IEnumerable<Movie>> GetMoviesByTypeAsync(string type)
        {
            List<Movie> movies = await _context.Movies.Where(m => m.Extension == type).OrderBy(movie => movie.Title).ToListAsync().ConfigureAwait(false);
            return movies;
        }
        public async Task<Movie> GetMovieAsync(string id)
        {
            Movie movie = await _context.Movies.FindAsync(id);
            return movie;
        }
        public async Task<Subtitle> GetSubtitleAsync(string id)
        {
            Subtitle subtitle = await _context.Subtitles.FindAsync(id);
            return subtitle;
        }
        public async Task<IEnumerable<Subtitle>> GetMovieSubtitlesAsync(string id)
        {
            Movie movie = await _context.Movies.FindAsync(id);
            return movie.Subtitles;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
