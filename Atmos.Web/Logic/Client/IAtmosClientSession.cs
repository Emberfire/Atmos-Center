using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Atmos.Web.Data.Entities;
using static Atmos.Web.Logic.Constants.CommonTypes;

namespace Atmos.Web.Logic.Client
{
    public interface IAtmosClientSession : IDisposable
    {
        string SessionId { get; set; }

        public FileInfo GetSingleFileOfType(DirectoryInfo directoryInfo, FileType fileType);
        public IEnumerable<FileInfo> GetFilesOfType(string path, FileType fileType);
        public IEnumerable<FileInfo> GetFilesOfType(string path, IEnumerable<FileType> fileTypes);
        public void RemoveFilesByPattern(string pattern, IEnumerable<FileInfo> files);

        public void RescanDirectoryForMovies(string path);
        public void RescanDirectoryForSubtitles(string path);
        public void PurgeMovies();

        public Task<IEnumerable<Movie>> GetAllMoviesAsync();
        public Task<IEnumerable<Movie>> GetMoviesByTypeAsync(string type);
        public Task<Movie> GetMovieAsync(string id);
        public Task<Subtitle> GetSubtitleAsync(string id);
        public Task<IEnumerable<Subtitle>> GetMovieSubtitlesAsync(string id);
    }
}