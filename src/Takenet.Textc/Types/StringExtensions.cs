using System;

namespace Takenet.Textc.Types
{
    public static class StringExtensions
    {
        /// <summary>
        /// Compute the editing distance between two strings.
        /// </summary>
        /// <param name="s">The first of the two strings.</param>
        /// <param name="t">The second of the two strings.</param>
        /// <returns>The Levenshtein cost.</returns>
        /// Source: http://rosettacode.org/wiki/Levenshtein_distance#C.23
        public static int CalculateLevenshteinDistance(this string s, string t)
        {
            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (var i = 0; i <= n; i++)
                d[i, 0] = i;
            for (var j = 0; j <= m; j++)
                d[0, j] = j;

            for (var j = 1; j <= m; j++)
            for (var i = 1; i <= n; i++)
                if (s[i - 1] == t[j - 1])
                    d[i, j] = d[i - 1, j - 1];  //no operation
                else
                    d[i, j] = Math.Min(Math.Min(
                            d[i - 1, j] + 1,    //a deletion
                            d[i, j - 1] + 1),   //an insertion
                        d[i - 1, j - 1] + 1 //a substitution
                    );
            return d[n, m];
        }
    }
}