using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Code_Challenge_9_MVC.Models
{
   
    public class MovieDb : DbContext
    {
        public MovieDb() : base("MoviesDB") { }

        public DbSet<Movie> Movies { get; set; }
    }
}

