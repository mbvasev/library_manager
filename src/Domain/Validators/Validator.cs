using Movies.Domain.Enums;
using Movies.Domain.Exceptions;
using Movies.Domain.Parsers;
using System;

namespace Movies.Domain.Validators.Movie
{
    internal enum StringState { Null, Empty, WhiteSpaces, Valid }

    internal static class Validator
    {
        public static void EnsureMovieIsValid(Movies.Domain.Models.Movie movie)
        {
            EnsureMovieIsNotNull(movie);
            var errorMessages = ValidateProperties(movie);
            EnsureNoErrors(errorMessages);
        }

        private static string ValidateProperties(Movies.Domain.Models.Movie movie)
        {
            var genreErrorMessage = ValidateGenre(movie.Genre);
            var titleErrorMessage = ValidateProperty("Title", movie.Title);
            var imageUrlErrorMessage = ValidateProperty("ImageUrl", movie.ImageUrl);
            var yearErrorMessage = ValidateYear(movie.Year);

            var errorMessages = genreErrorMessage + titleErrorMessage + imageUrlErrorMessage + yearErrorMessage;
            return errorMessages.Length == 0 ? null : errorMessages;
        }

        private static void EnsureNoErrors(string errorMessages)
        {
            if (errorMessages != null)
            {
                throw new InvalidMovieException(errorMessages);
            }
        }

        private static void EnsureMovieIsNotNull(Movies.Domain.Models.Movie movie)
        {
            if (movie == null)
            {
                throw new InvalidMovieException("The movie parameter can not be null.");
            }
        }

        private static string ValidateYear(int year)
        {
            const int minimumYear = 1900;

            if (year >= minimumYear && year <= DateTime.Today.Year)
            {
                return null;
            }

            return $"The Year, must be between {minimumYear} and {DateTime.Today.Year} (inclusive)";
        }

        private static string ValidateGenre(Genre genre)
        {
            var errorMessage = GenreParser.ValidationMessage(genre);
            if (errorMessage != null)
            {
                errorMessage += "\r\n";
            }

            return errorMessage;
        }

        private static string ValidateProperty(string propertyName, string propertyValue)
        {
            switch (DetermineNullEmptyOrWhiteSpaces(propertyValue))
            {
                case StringState.Null:
                    return $"The Movie {propertyName} must be a valid {propertyName} and can not be null\r\n";
                case StringState.Empty:
                    return $"The Movie {propertyName} must be a valid {propertyName} and can not be Empty\r\n";
                case StringState.WhiteSpaces:
                    return $"The Movie {propertyName} must be a valid {propertyName} and can not be Whitespaces\r\n";
                case StringState.Valid:
                default:
                    return null;
            }
        }

        private static StringState DetermineNullEmptyOrWhiteSpaces(string data)
        {
            if (data == null)
            {
                return StringState.Null;
            }
            else if (data.Length == 0)
            {
                return StringState.Empty;
            }

            foreach (var chr in data)
            {
                if (!char.IsWhiteSpace(chr))
                {
                    return StringState.Valid;
                }
            }

            return StringState.WhiteSpaces;
        }
    }
}