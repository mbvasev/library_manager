using Microsoft.EntityFrameworkCore;
using Movies.Domain.Models;
using Movies.Domain.ServiceLocators;

namespace Movies.Domain.Data
{
    public class MoviesDbContext : DbContext
    {
        private readonly string _dbConnString;

        public MoviesDbContext(string dbConnString)
        {
            _dbConnString = dbConnString;
        }

        public MoviesDbContext()
        {
            _dbConnString = new ServiceLocator().CreateConfigurationProvider().GetDbConnectionString();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_dbConnString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #pragma warning disable CA1062
            modelBuilder.Entity<Movie>(
                b =>
                {
                    b.Property(e => e.Title);
                    b.Property(e => e.ImageUrl);
                    b.Property(e => e.Genre);
                    b.Property(e => e.Year);
                    b.HasIndex(e => e.Title).IsUnique();
                    b.Property(p => p.TimeStamp).IsRowVersion();
                });
        }

        public DbSet<Movie> Movies { get; set; }
    }
}
