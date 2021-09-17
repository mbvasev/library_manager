using Movies.Domain;
using Movies.Domain.Models;
using Movies.Domain.Parsers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Service.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DomainFacade _domainFacade;

        private IEnumerable<Movie> _movies;
        public IEnumerable<Movie> Movies { get { return _movies; } }

        [BindProperty(SupportsGet = true)]
        public string Genre { get; set; }

        public IndexModel(DomainFacade domainFacade, ILogger<IndexModel> logger)
        {
            _domainFacade = domainFacade;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            _movies = string.IsNullOrEmpty(Genre) ? await _domainFacade.GetAllMovies() : await _domainFacade.GetMoviesByGenre(GenreParser.Parse(Genre));
        }
    }
}
