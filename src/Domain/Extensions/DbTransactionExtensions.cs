using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Movies.Domain.Extensions
{
    internal static class DbTransactionExtensions
    {
        [ExcludeFromCodeCoverage]
        public static void RollbackIfNotNull(this DbTransaction dbTransaction)
        {
            if (dbTransaction != null)
            {
                dbTransaction.Rollback();
            }
        }
    }
}
