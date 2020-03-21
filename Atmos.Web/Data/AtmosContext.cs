using Atmos.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Data Source=DESKTOP-S06O8U5;Initial Catalog=AtmosDb;Integrated Security=True");
        //}
    }
}
