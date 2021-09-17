using Movies.Domain.Enums;
using Movies.Domain.Models;
using System.Collections.Generic;
using Test.Shared.Helpers;

namespace Test.Shared
{
    public static class RandomMovieGenerator
    {
        public static IEnumerable<Movie> GenerateRandomMovies(int count)
        {
            var randomMovies = new List<Movie>();
            for (int i = 0; i < count; i++)
            {
                randomMovies.Add(new Movie
                    (
                        title: RandomGenerator.GetRandomAciiString(50),
                        imageUrl: RandomGenerator.GetRandomAciiString(200),
                        genre: (Genre)RandomGenerator.GetRandomInt(0, 5),
                        year: RandomGenerator.GetRandomInt(1980, 2020)
                    ));
            }

            return randomMovies;
        }
    }
}
