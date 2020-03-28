using Atmos.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Atmos.Web.Logic.Utils
{
    /// <summary>
    /// Custom comparer for the <see cref="Movie"/> class
    /// </summary>
    public class MovieComparer : IEqualityComparer<Movie>
    {
        public bool Equals([AllowNull] Movie x, [AllowNull] Movie y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            // Check whether any of the compared objects is null.
            if (x is null || y is null)
            {
                return false;
            }

            // Check whether the movies' properties are equal.
            return x.Title == y.Title && x.Path == y.Path && x.Extension == y.Extension;
        }

        public int GetHashCode([DisallowNull] Movie movie)
        {
            if (movie is null)
            {
                throw new ArgumentNullException(nameof(movie));
            }

            return string.GetHashCode(movie.Id, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
