using Atmos.Web.Logic.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Atmos.Web.Data.Configurations;
using Atmos.Web.Data.Entities;

namespace Atmos.Web.Data
{
    public class AtmosContext : DbContext
    {
        public AtmosContext()
        {
        }

        public AtmosContext(DbContextOptions<AtmosContext> options) : base(options)
        {
        }

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Subtitle> Subtitles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new SubtitleConfiguration());
        }

        public void UpdateMovie(Movie movie)
        {
            if (movie is null)
            {
                throw new ArgumentNullException(nameof(movie));
            }

            Movie existingMovie = Movies.Find(movie.Title);
            if (existingMovie != null)
            {
                if (movie.Path != existingMovie.Path)
                {
                    existingMovie.Path = movie.Path;
                }
                if (movie.Extension != existingMovie.Extension)
                {
                    existingMovie.Extension = movie.Extension;
                }
                if (movie.Title != existingMovie.Title)
                {
                    existingMovie.Title = movie.Title;
                }

                SubtitleComparer comparer = new SubtitleComparer();
                foreach (Subtitle subtitle in movie.Subtitles)
                {
                    Subtitle existingSubtitle = existingMovie.Subtitles.FirstOrDefault(s => s.Id == subtitle.Id);
                    if (existingSubtitle != null && !comparer.Equals(subtitle, existingSubtitle))
                    {
                        existingMovie.Subtitles.Remove(existingSubtitle);
                        Subtitles.Remove(existingSubtitle);
                        Subtitles.Add(subtitle);
                        existingMovie.Subtitles.Add(subtitle);
                    }
                }
            }
        }

        public void UpdateSubtitle(Subtitle subtitle)
        {
            if (subtitle is null)
            {
                throw new ArgumentNullException(nameof(subtitle));
            }

            Subtitle existingSubtitle = Subtitles.FirstOrDefault(s => s.Path == subtitle.Path);
            if (existingSubtitle != null)
            {
                if (subtitle.Path != existingSubtitle.Path)
                {
                    existingSubtitle.Path = subtitle.Path;
                }
                if (subtitle.Language != existingSubtitle.Language)
                {
                    existingSubtitle.Language = subtitle.Language;
                }

                MovieComparer comparer = new MovieComparer();
                if (!comparer.Equals(subtitle.Movie, existingSubtitle.Movie))
                {
                    Movie movie = existingSubtitle.Movie;
                    existingSubtitle.Movie = subtitle.Movie;
                    movie.Subtitles.Remove(existingSubtitle);
                    movie.Subtitles.Add(subtitle);
                }
            }
        }
    }
}
