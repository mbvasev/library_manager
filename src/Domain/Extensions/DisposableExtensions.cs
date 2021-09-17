using System;
using System.Diagnostics.CodeAnalysis;

namespace Movies.Domain.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class DisposableExtensions
    {
        public static void DisposeIfNotNull(this IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            disposable.Dispose();
        }
    }
}
