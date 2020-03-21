using Atmos.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Atmos.Web.Logic.Client
{
    public interface IAtmosClientSession : IDisposable
    {
        string SessionId { get; set; }

        public void ScanFolderForMovies(string folderPath, SearchOption searchOption);
        public Task<List<Movie>> GetAllMoviesAsync();
        public Task<Movie> GetMovieAsync(string id);
    }
}