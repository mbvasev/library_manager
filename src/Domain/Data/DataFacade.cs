using Movies.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Domain.Data.Managers
{
    internal sealed class DataFacade
    {
        private readonly string _dbConnectionString;
        private Movie.DataManager _movieDataManager;

        private Movie.DataManager MovieDataManager { get { return _movieDataManager ?? (_movieDataManager = new Movie.DataManager(_dbConnectionString)); } }

        public DataFacade(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }

        public Task<int> CreateMovie(Models.Movie movie)
        {
            return MovieDataManager.CreateMovie(movie);
        }

        public Task<Models.Movie> GetMovieById(int id)
        {
            return MovieDataManager.GetMovieById(id);
        }

        public Task<IEnumerable<Models.Movie>> GetMovieByGenre(Genre genre)
        {
            return MovieDataManager.GetMovieByGenre(genre);
        }

        public Task<IEnumerable<Models.Movie>> GetAllMovies()
        {
            return MovieDataManager.GetAllMovies();
        }
    }
}
