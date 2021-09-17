using Movies.Domain.Models;
using System.Collections.Generic;

namespace Test.Acceptance.Mediators
{
    internal sealed class Mediator
    {
        public IEnumerable<Movie> MoviesForGetAllMovies { get; set; }
        public ExceptionInformation ExceptionInformation { get; set; }
    }

    internal sealed class ExceptionInformation
    {
        public ExceptionReason ExceptionReason { get; set; }
    }

    internal enum ExceptionReason { NotFond = 404, BadRequest = 400, ProxyAuthenticationRequired = 407, BadGateway = 502, ServiceUnavailable = 503 }
}
