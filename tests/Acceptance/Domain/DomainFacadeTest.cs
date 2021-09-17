using Test.Acceptance.Domain.ServiceLocators;
using Test.Acceptance.DataGenerators;
using Test.Acceptance.Mediators;
using Movies.Domain;
using Movies.Domain.Enums;
using Movies.Domain.Exceptions;
using Movies.Domain.Models;
using Movies.Domain.Services.ImdbService.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Shared;
using Test.Shared.Helpers;
using Movies.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Test.Acceptance.Domain.ServiceLocators
{
    [TestClass]
    public class DomainFacadeTests
    {
        private readonly IEnumerable<Movie> moviesInDb;
        private readonly string dbConnectionString;

        public DomainFacadeTests()
        {

            dbConnectionString = new ServiceLocatorForAcceptanceTesting(null).CreateConfigurationProvider().GetDbConnectionString();
            moviesInDb = MovieTestDataGenerator.GetAllMovies(dbConnectionString).GetAwaiter().GetResult();
            
        }

        private (DomainFacade domainFacade, Mediator testMediator) CreateDomainFacade()
        {
            var testMediator = new Mediator();
            var serviceLocatorForAcceptanceTesting = new ServiceLocatorForAcceptanceTesting(testMediator);
            return (new DomainFacade(serviceLocatorForAcceptanceTesting), testMediator);
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            var dbConnectionString = new ServiceLocatorForAcceptanceTesting(null).CreateConfigurationProvider().GetDbConnectionString();
            using (var dbConext = new MoviesDbContext(dbConnectionString))
            {
                dbConext.Database.EnsureDeleted();
                dbConext.Database.Migrate();
            }
        }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var dbConnectionString = new ServiceLocatorForAcceptanceTesting(null).CreateConfigurationProvider().GetDbConnectionString();
            using (var dbConext = new MoviesDbContext(dbConnectionString))
            {
                 var movies = RandomMovieGenerator.GenerateRandomMovies(5);
                dbConext.Movies.AddRange(movies);
                dbConext.SaveChanges();
                
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task GetAllMovies_WhenCalled_ReturnsAllMovies()
        {
            // Arrange
            var (domainFacade, testMediator) = CreateDomainFacade();
            try
            {
                var moviesFromService = RandomMovieGenerator.GenerateRandomMovies(50);
                testMediator.MoviesForGetAllMovies = moviesFromService;

                var expectedMovies = new List<Movie>();
                expectedMovies.AddRange(moviesFromService);
                expectedMovies.AddRange(moviesInDb);

                // Act
                var actualMovies = await domainFacade.GetAllMovies();

                // Assert
                MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task GetMoviesByGenre_WhenCalledWithValidGenre_ShouldReturnMoviesFilteredByGenre()
        {
            // Arrange
            var (domainFacade, testMediator) = CreateDomainFacade();
            try
            {
                var randomMovies = new List<Movie>();

                var moviesFromService = RandomMovieGenerator.GenerateRandomMovies(50);
                testMediator.MoviesForGetAllMovies = moviesFromService;

                randomMovies.AddRange(moviesFromService);
                randomMovies.AddRange(moviesInDb);
                var expectGenre = Genre.Comedy;
                var expectedMovies = randomMovies.Where(m => m.Genre == expectGenre).ToList();

                // Act
                var actualMovies = await domainFacade.GetMoviesByGenre(expectGenre);

                // Assert
                MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task GetMoviesById_WhenCalledWithValidId_ShouldReturnMovieMatchingId()
        {
            // Arrange
            var (domainFacade, testMediator) = CreateDomainFacade();
            try
            {
                var expectedMovie = moviesInDb.First();
                var expectedMovieId = await MovieTestDataGenerator.GetMovieIdByTitle(dbConnectionString, expectedMovie.Title);

                // Act
                var actualMovie = await domainFacade.GetMovieById(expectedMovieId);

                // Assert
                MovieAssertions.AssertMoviesAreEqual(new[] { expectedMovie }, new[] { actualMovie });
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task GetMoviesById_WhenCalledWithANonExistantMovieId_ShouldThrow()
        {
            // Arrange
            var (domainFacade, testMediator) = CreateDomainFacade();
            var nonExistantMovieId = -1;

            try
            {
                // Act
                _ = await domainFacade.GetMovieById(nonExistantMovieId);
                Assert.Fail("We Were Expecting a MovieWithSpecifiedIdNotFoundException to be thrown, but no exception was thrown.");
            }
            catch (MovieWithSpecifiedIdNotFoundException e)
            {
                // Assert
                StringAssert.Contains(e.Message, $"Id: {nonExistantMovieId}");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task GetMoviesByGenre_WhenCalledWithAnInvalidGenre_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            Genre invalidGenre = (Genre)1000;
            try
            {
                // Act
                await domainFacade.GetMoviesByGenre(invalidGenre);
                Assert.Fail("We were expecting an InvalidGenreException expection to be thrown but no exception was thrown");
            }
            catch (InvalidGenreException e)
            {
                StringAssert.Contains(e.Message, $"{(int)invalidGenre} is not a valid Genre");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task GetAllMovies_ServiceEndpointIsNotFound_ShouldThrow()
        {
            // Arrange
            var (domainFacade, testMediator) = CreateDomainFacade();
            try
            {
                testMediator.ExceptionInformation
                    = new ExceptionInformation { ExceptionReason = ExceptionReason.NotFond };

                // Act
                await domainFacade.GetAllMovies();
                Assert.Fail("We were expecting an exception of type ImdbServiceNotFoundException to be thrown, but no exception was thrown");
            }
            catch (ImdbServiceNotFoundException e)
            {
                // Assert
                AssertEx.EnsureExceptionMessageContains(e, "resulted in a Not Found Status Code");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task GetAllMovies_WhenProxyAuthenticationFailed_ShouldThrow()
        {
            // Arrange
            var (domainFacade, testMediator) = CreateDomainFacade();
            try
            {
                testMediator.ExceptionInformation
                    = new ExceptionInformation { ExceptionReason = ExceptionReason.ProxyAuthenticationRequired };

                // Act
                await domainFacade.GetAllMovies();
                Assert.Fail("We were expecting an exception of type ImdbProxyAuthenticationRequiredException to be thrown, but no exception was thrown");
            }
            catch (ImdbProxyAuthenticationRequiredException e)
            {
                // Assert
                AssertEx.EnsureExceptionMessageContains(e, "Imdb Service", "status code", "ProxyAuthenticationRequired");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }


        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task GetAllMovies_WhenServiceIsNotAvailable_ShouldThrow()
        {
            // Arrange
            var (domainFacade, testMediator) = CreateDomainFacade();
            try
            {
                testMediator.ExceptionInformation
                    = new ExceptionInformation { ExceptionReason = ExceptionReason.ServiceUnavailable };

                // Act
                await domainFacade.GetAllMovies();
                Assert.Fail("We were expecting an exception of type ImdbServiceNotFoundException to be thrown, but no exception was thrown");
            }
            catch (ImdbServiceNotFoundException e)
            {
                // Assert
                AssertEx.EnsureExceptionMessageContains(e, "Imdb Service", "status code", "ServiceUnavailable");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenCalledWithAValidMovieNonExistent_Succeed()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            var expectedMovie = RandomMovieGenerator.GenerateRandomMovies(1).Single();
            try
            {
                // Act
                await domainFacade.CreateMovie(expectedMovie);

                // Assert
                var actualMovie = await MovieTestDataGenerator.RetrieveMovie(dbConnectionString, expectedMovie.Title);
                MovieAssertions.AssertMoviesAreEqual(new[] { expectedMovie }, new[] { actualMovie });
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenDuplicateMovieIsCreated_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            var expectedMovie = RandomMovieGenerator.GenerateRandomMovies(1).Single();
            try
            {
                // Act
                await domainFacade.CreateMovie(expectedMovie);
                await domainFacade.CreateMovie(new Movie(title: expectedMovie.Title, imageUrl: expectedMovie.ImageUrl, genre: expectedMovie.Genre, year: expectedMovie.Year));
                Assert.Fail("We were expecting a DuplicateMovieException exception to be thrown, but no exception was thrown");

            }
            catch (DuplicateMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, $"Title: {expectedMovie.Title} already exists");
            }

            catch (Exception e)
            {
                Assert.Fail($"We were expecting a {typeof(DuplicateMovieException)} exception to be thrown, but exception {e.GetType()} was thrown");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }


        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieIsNull_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            Movie nullMovie = null;
            try
            {
                // Act
                await domainFacade.CreateMovie(nullMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                AssertEx.EnsureExceptionMessageContains(e, "movie", "parameter can not be null.");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieTitleIsNull_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: null, imageUrl: "http://someurl", genre: Genre.Action, year: 1900);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, " valid Title and can not be null");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "ImageUrl", "Genre", "Year");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieTitleIsEmpty_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: string.Empty, imageUrl: "http://someurl", genre: Genre.Action, year: 1900);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, " valid Title and can not be Empty");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "ImageUrl", "Genre", "Year");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieTitleIsWhitespaces_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: "    ", imageUrl: "http://someurl", genre: Genre.Action, year: 1900);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, " valid Title and can not be Whitespaces");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "ImageUrl", "Genre", "Year");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieImageUrlIsNull_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: RandomGenerator.GetRandomAciiString(50), imageUrl: null, genre: Genre.Action, year: 1900);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, " valid ImageUrl and can not be null");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "Genre", "Year");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieImageUrlIsEmpty_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: RandomGenerator.GetRandomAciiString(50), imageUrl: string.Empty, genre: Genre.Action, year: 1900);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, " valid ImageUrl and can not be Empty");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "Genre", "Year");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieImageUrlIsWhitespaces_ShouldThrow()
        {
            // Arrange
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: RandomGenerator.GetRandomAciiString(50), imageUrl: "    ", genre: Genre.Action, year: 1900);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, " valid ImageUrl and can not be Whitespaces");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "Genre", "Year");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieYearIsLessThan1900_ShouldThrow()
        {
            // Arrange
            var minimumYear = 1900;
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: RandomGenerator.GetRandomAciiString(50), imageUrl: "http://someurl", genre: Genre.Action, year: minimumYear - 1);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, $"The Year, must be between {minimumYear} and {DateTime.Today.Year}");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "ImageUrl", "Genre");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieYearIsGreaterThanTodaysYear_ShouldThrow()
        {
            // Arrange
            var todaysYear = DateTime.Today.Year;
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: RandomGenerator.GetRandomAciiString(50), imageUrl: "http://someurl", genre: Genre.Action, year: todaysYear + 1);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, $"The Year, must be between 1900 and {DateTime.Today.Year}");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "ImageUrl", "Genre");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task CreateMovie_WhenMovieGenreIsNotValid_ShouldThrow()
        {
            // Arrange
            var invalidGenre = (Genre)1000;
            var todaysYear = DateTime.Today.Year;
            var (domainFacade, _) = CreateDomainFacade();
            var invalidMovie = new Movie(title: RandomGenerator.GetRandomAciiString(50), imageUrl: "http://someurl", genre: invalidGenre, year: todaysYear);
            try
            {
                // Act
                await domainFacade.CreateMovie(invalidMovie);
                Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");

            }
            catch (InvalidMovieException e)
            {
                // Assert
                StringAssert.Contains(e.Message, $"The Genre: {invalidGenre} is not a valid Genre");
                AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "ImageUrl", "Year");
            }
            finally
            {
                domainFacade.Dispose();
            }
        }
    }
}
