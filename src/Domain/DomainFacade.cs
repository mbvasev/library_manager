using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Movies.Domain.Enums;
using Movies.Domain.Models;
using Movies.Domain.ServiceLocators;

[assembly: InternalsVisibleTo("Test.Acceptance")]
[assembly: InternalsVisibleTo("Test.Shared")]
[assembly: InternalsVisibleTo("Test.Class")]

namespace Movies.Domain
{
    public sealed class DomainFacade : IDisposable
    {
        private bool _disposed;
        private readonly ServiceLocatorBase _serviceLocator;

        private Managers.Movie.Manager _movieManager;

        private Managers.Movie.Manager MovieManager { get { return _movieManager ??= new Managers.Movie.Manager(_serviceLocator); } }

        public DomainFacade()
          : this(new ServiceLocator())
        {
        }

        internal DomainFacade(ServiceLocatorBase serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public Task<Movie> GetMovieById(int id)
        {
            return MovieManager.GetMovieById(id);
        }

        public Task<IEnumerable<Movie>> GetAllMovies()
        {
            return MovieManager.GetAllMovies();
        }

        public Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
        {
            return MovieManager.GetMoviesByGenre(genre);
        }

        public Task<int> CreateMovie(Movie movie)
        {
            return MovieManager.CreateMovie(movie);
        }

        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed && _movieManager != null)
            {
                var localMovieManager = _movieManager;
                localMovieManager.Dispose();
                _movieManager = null;
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
