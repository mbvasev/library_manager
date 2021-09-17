using Movies.Domain;
using Movies.Domain.Enums;
using Movies.Domain.Models;
using Movies.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.Controller
{
    internal sealed class MovieControllerForTest : MoviesController
    {
        private readonly IEnumerable<Movie> _movies;
        private readonly Exception _exception;

        public MovieControllerForTest(DomainFacade domainFacade)
            : base(domainFacade)
        {
        }

        public MovieControllerForTest(IEnumerable<Movie> movies)
            : base(null)
        {
            _movies = movies;
        }

        public MovieControllerForTest(Exception exception)
            : base(null)
        {
            _exception = exception;
        }

        protected override Task<IEnumerable<Movie>> GetAllMovies()
        {
            if (_exception != null)
            {
                throw _exception;
            }

            return Task.FromResult(_movies);
        }

        protected override Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
        {
            return Task.FromResult(_movies);
        }

        protected override Task<int> CreateMovie(Movie movie)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            return Task.FromResult(0);
        }
    }
}
