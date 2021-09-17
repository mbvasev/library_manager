using Movies.Domain.ConfigurationProviders;
using Movies.Domain.Data.Managers;
using Movies.Domain.Enums;
using Movies.Domain.Exceptions;
using Movies.Domain.Models;
using Movies.Domain.Parsers;
using Movies.Domain.ServiceLocators;
using Movies.Domain.Services.ImdbService;
using Movies.Domain.Validators.Movie;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Domain.Managers.Movie
{
    internal sealed class Manager : IDisposable
    {
        private bool _disposed;
        private readonly ServiceLocatorBase _serviceLocator;

        private ConfigurationProviderBase _configurationProvider;
        private ConfigurationProviderBase ConfigurationProvider { get { return _configurationProvider ?? (_configurationProvider = _serviceLocator.CreateConfigurationProvider()); } }

        private ImdbServiceGateway _imdbServiceGateway;
        private ImdbServiceGateway ImdbServiceGateway
        {
            get
            {
                return _imdbServiceGateway ?? (_imdbServiceGateway = _serviceLocator.CreateImdbServiceGateway());
            }
        }

        private DataFacade _dataFacade;
        private DataFacade DataFacade { get { return _dataFacade ?? (_dataFacade = new DataFacade(ConfigurationProvider.GetDbConnectionString())); } }

        public Manager(ServiceLocatorBase serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public Task<int> CreateMovie(Models.Movie movie)
        {
            Validator.EnsureMovieIsValid(movie);
            return DataFacade.CreateMovie(movie);
        }

        public async Task<Models.Movie> GetMovieById(int id)
        {
            var movie = await DataFacade.GetMovieById(id).ConfigureAwait(false);
            return movie ?? throw new MovieWithSpecifiedIdNotFoundException($"A Movie with Id: {id} was Not Found");
        }

        public Task<IEnumerable<Models.Movie>> GetAllMovies()
        {
            var moviesTask = ImdbServiceGateway.GetAllMovies();
            var moviesFromDbTask = DataFacade.GetAllMovies();
            return GetMoviesFromCombinedTasks(moviesTask, moviesFromDbTask);
        }

        private static async Task<IEnumerable<Models.Movie>> GetMoviesFromCombinedTasks(Task<IEnumerable<Models.Movie>> moviesTask, Task<IEnumerable<Models.Movie>> moviesFromDbTask)
        {
            await Task.WhenAll(moviesTask, moviesFromDbTask).ConfigureAwait(false);

            var movies = moviesTask.Result;
            var moviesFromDb = moviesFromDbTask.Result;

            var moviesList = movies.ToList();
            moviesList.AddRange(moviesFromDb);
            return moviesList;
        }

        public async Task<IEnumerable<Models.Movie>> GetMoviesByGenre(Genre genre)
        {
            GenreParser.Validate(genre);
            var moviesFromImdbTask = ImdbServiceGateway.GetAllMovies();
            var moviesFromDbMatchingGenreTask = DataFacade.GetMovieByGenre(genre);
            await Task.WhenAll(moviesFromImdbTask, moviesFromDbMatchingGenreTask).ConfigureAwait(false);

            var moviesMatchingGenre = moviesFromDbMatchingGenreTask.Result.ToList();
            moviesMatchingGenre.AddRange(moviesFromImdbTask.Result.Where(m => m.Genre == genre));
            return moviesMatchingGenre;
        }

        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed && _imdbServiceGateway != null)
            {
                var localImdbServiceGateway = _imdbServiceGateway;
                localImdbServiceGateway.Dispose();
                _imdbServiceGateway = null;
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
