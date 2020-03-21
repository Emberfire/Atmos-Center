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

        List<FileInfo> GetFilesOfType(DirectoryInfo directoryInfo, CommonTypes.MovieFileType fileType, SearchOption searchOption)
        {
            string searchPattern = fileType switch
            {
                CommonTypes.MovieFileType.Mp4 => "*.mp4",
                CommonTypes.MovieFileType.Mkv => "*.mkv",
                CommonTypes.MovieFileType.Avi => "*.avi",
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
            files.AddRange(GetFilesOfType(directoryInfo, CommonTypes.MovieFileType.Mp4, searchOption));
            files.AddRange(GetFilesOfType(directoryInfo, CommonTypes.MovieFileType.Mkv, searchOption));
            files.AddRange(GetFilesOfType(directoryInfo, CommonTypes.MovieFileType.Avi, searchOption));

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

            using AtmosContext context = new AtmosContext();
            foreach (Movie movie in movies)
            {
                bool alreadyExists = context.Movies.Any(m => m.Title == movie.Title);
                if (!alreadyExists)
                {
                    context.Movies.Add(movie);
                }
            }
            
            context.SaveChanges();
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
