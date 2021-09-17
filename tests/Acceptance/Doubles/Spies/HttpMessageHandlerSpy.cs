using Test.Acceptance.Mediators;
using Movies.Domain.Parsers;
using Movies.Domain.Services.ImdbService.ResourceModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Acceptance.Doubles.Spies
{
    internal sealed class HttpMessageHandlerSpy : HttpMessageHandler
    {
        private readonly Mediator _testMediator;

        public HttpMessageHandlerSpy(Mediator testMeditor)
        {
            _testMediator = testMeditor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_testMediator.ExceptionInformation == null)
            {
                var movies = _testMediator.MoviesForGetAllMovies;
                return WriteMoviesToResponse(request, movies);
            }
            else
            {
                var httpResponseMessage = request.CreateResponse();
                httpResponseMessage.StatusCode = (HttpStatusCode)_testMediator.ExceptionInformation.ExceptionReason;
                httpResponseMessage.Content = new StringContent("Exception Occured");
                return Task.FromResult(httpResponseMessage);
            }
        }

        private static Task<HttpResponseMessage> WriteMoviesToResponse(HttpRequestMessage request, IEnumerable<Movies.Domain.Models.Movie> movies)
        {
            IEnumerable<ImdbMovie> imdbMovies = null;

            var absolutePath = request.RequestUri.AbsolutePath;

            if (absolutePath.Contains("withcategories"))
            {
                imdbMovies = movies.Select(m => new ImdbMovie { Title = m.Title, Category = GenreParser.ToString(m.Genre) });
            }
            else if (absolutePath.Contains("withimageurls"))
            {
                imdbMovies = movies.Select(m => new ImdbMovie { Title = m.Title, ImageUrl = m.ImageUrl });
            }
            else if (absolutePath.Contains("withyears"))
            {
                imdbMovies = movies.Select(m => new ImdbMovie { Title = m.Title, Year = m.Year });
            }

            var imdbMoviesJson = JsonConvert.SerializeObject(imdbMovies);
            var httpResponseMessage = request.CreateResponse();
            httpResponseMessage.Content = new StringContent(imdbMoviesJson, Encoding.UTF8, "application/json");

            return Task.FromResult(httpResponseMessage);
        }
    }
}
