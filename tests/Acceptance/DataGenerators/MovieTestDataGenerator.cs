using Movies.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Acceptance.DataGenerators
{
    internal static class MovieTestDataGenerator
    {
        public static async Task<IEnumerable<Movie>> GetAllMovies(string dbConnectionString)
        {
            using (var dbContext = new Movies.Domain.Data.MoviesDbContext(dbConnectionString))
            {
                return await Task.FromResult(dbContext.Movies.ToList<Movie>());
            }
        }

        public static async Task<int> GetMovieIdByTitle(string dbConnectionString, string title)
        {

            using (var dbContext = new Movies.Domain.Data.MoviesDbContext(dbConnectionString))
            {

                return await Task.FromResult(dbContext.Movies.Single(m => m.Title == title).Id);
            }
        }


        public static async Task<Movie> RetrieveMovie(string dbConnectionString, string title)
        {
            using (var dbContext = new Movies.Domain.Data.MoviesDbContext(dbConnectionString))
            {

                return await Task.FromResult(dbContext.Movies.Single(m => m.Title == title));
            }
        }
    }
}
