using System.ComponentModel.DataAnnotations;
using Movies.Domain.Enums;

namespace Movies.Domain.Models
{
    public sealed class Movie
    {
        public int Id { get; private set; }
        public string Title { get; }
        public string ImageUrl { get; }
        public Genre Genre { get; }
        public int Year { get; }

        [Timestamp]
        public byte[] TimeStamp { get; private set;}

        public Movie(string title, string imageUrl, Genre genre, int year)
        {
            Title = title;
            ImageUrl = imageUrl;
            Genre = genre;
            Year = year;
        }
    }
}
