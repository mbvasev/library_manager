using Microsoft.EntityFrameworkCore;
using Movies.Domain.Enums;
using Movies.Domain.Exceptions;
using Movies.Domain.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Domain.Data.Managers.Movie
{
    internal sealed class DataManager
    {
        private readonly string _dbConnectionString;

        public DataManager(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }

        public async Task<int> CreateMovie(Movies.Domain.Models.Movie movie)
        {
            using (var moviesDbContext = new MoviesDbContext(_dbConnectionString))
            {
                try
                {
                    moviesDbContext.Movies.Add(movie);
                    return await moviesDbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException != null)
                    {
                        if (e.InnerException.Message.Contains("duplicate key row in object 'dbo.Movies'", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new DuplicateMovieException($"A Movie with the Title: {movie.Title} already exists. Please use a different title", e);
                        }

                        if (e.InnerException.Message.Contains("Cannot insert", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidMovieException(e.InnerException.Message);
                        }

                        throw;
                    }

                    throw;
                }
            }
        }

        public async Task CreateMovies(IEnumerable<Movies.Domain.Models.Movie> movies)
        {
            using (var moviesDbContext = new MoviesDbContext(_dbConnectionString))
            {
                moviesDbContext.Movies.AddRange(movies);
                await moviesDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<Movies.Domain.Models.Movie> GetMovieById(int id)
        {
            using (var moviesDbContext = new MoviesDbContext(_dbConnectionString))
            {
                return await moviesDbContext.FindAsync<Movies.Domain.Models.Movie>(id);
            }
        }

        public async Task<IEnumerable<Movies.Domain.Models.Movie>> GetMovieByGenre(Genre genre)
        {
            using (var moviesDbContext = new MoviesDbContext(_dbConnectionString))
            {
                return await Task.FromResult(moviesDbContext.Movies.Where(m => m.Genre == genre).ToList()).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Movies.Domain.Models.Movie>> GetAllMovies()
        {
            using (var moviesDbContext = new MoviesDbContext(_dbConnectionString))
            {
                return await Task.FromResult(moviesDbContext.Movies.ToList()).ConfigureAwait(false);
            }
        }

        private static async Task<Movies.Domain.Models.Movie> MapToMovie(DbDataReader dbDataReader)
        {
            if (await dbDataReader.ReadAsync().ConfigureAwait(false))
            {
                return new Movies.Domain.Models.Movie(
                    title: (string)dbDataReader[0],
                    genre: GenreParser.Parse((string)dbDataReader[1]),
                    year: (int)dbDataReader[2],
                    imageUrl: (string)dbDataReader[3]);
            }

            return null;
        }

        private static async Task<IEnumerable<Movies.Domain.Models.Movie>> MapToMovies(DbDataReader dbDataReader)
        {
            var movies = new List<Movies.Domain.Models.Movie>();

            while (await dbDataReader.ReadAsync().ConfigureAwait(false))
            {
                movies.Add(new Movies.Domain.Models.Movie(
                    title: (string)dbDataReader[0],
                    genre: GenreParser.Parse((string)dbDataReader[1]),
                    year: (int)dbDataReader[2],
                    imageUrl: (string)dbDataReader[3]));
            }

            return movies;
        }
    }
}
