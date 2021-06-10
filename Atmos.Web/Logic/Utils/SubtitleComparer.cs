using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Atmos.Web.Data.Entities;

namespace Atmos.Web.Logic.Utils
{
    /// <summary>
    /// Custom comparer for the <see cref="Subtitle"/> class
    /// </summary>
    public class SubtitleComparer : IEqualityComparer<Subtitle>
    {
        public bool Equals([AllowNull] Subtitle x, [AllowNull] Subtitle y)
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

            // Check whether the subtitles' properties are equal.
            MovieComparer comparer = new MovieComparer();
            return x.Path == y.Path && x.Language == y.Language && comparer.Equals(x.Movie, y.Movie);
        }

        public int GetHashCode([DisallowNull] Subtitle subtitle)
        {
            if (subtitle is null)
            {
                throw new ArgumentNullException(nameof(subtitle));
            }

            return string.GetHashCode(subtitle.Id, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}