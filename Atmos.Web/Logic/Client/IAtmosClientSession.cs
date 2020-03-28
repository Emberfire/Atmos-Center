using Atmos.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static Atmos.Web.Logic.Constants.CommonTypes;

namespace Atmos.Web.Logic.Client
{
    public interface IAtmosClientSession : IDisposable
    {
        string SessionId { get; set; }

        public FileInfo GetSingleFileOfType(DirectoryInfo directoryInfo, FileType fileType);
        public List<FileInfo> GetFilesOfType(string path, FileType fileType);
        public List<FileInfo> GetFilesOfType(string path, IEnumerable<FileType> fileTypes);
        public void RemoveFilesByPattern(string pattern, List<FileInfo> files);

        public void RescanDirectoryForMovies(string path);
        public void RescanDirectoryForSubtitles(string path);
        public void PurgeMovies();

        public Task<List<Movie>> GetAllMoviesAsync();
        public Task<List<Movie>> GetMoviesByTypeAsync(string type);
        public Task<Movie> GetMovieAsync(string id);
        public Task<Subtitle> GetSubtitleAsync(string id);
        public Task<List<Subtitle>> GetMovieSubtitlesAsync(string id);
    }
}