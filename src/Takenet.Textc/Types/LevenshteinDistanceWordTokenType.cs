using System;
using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    [TokenType(ShortName = "LDWord")]
    public class LevenshteinDistanceWordTokenType : ValueTokenTypeBase<string>
    {
        public LevenshteinDistanceWordTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
            MaxDistance = 2;
        }

        [TokenTypeProperty]
        public int MaxDistance { get; internal set; }

        protected override bool HasMatch(string value, IRequestContext context, out string bestMatch)
        {
            var match = false;
            bestMatch = null;

            var bestDistance = int.MaxValue;

            foreach (var validValue in GetValidValues(context))
            {
                var distance = LevenshteinDistance(value, validValue);

                if (distance >= 0 &&
                    distance <= MaxDistance &&
                    distance < bestDistance)
                {
                    match = true;

                    bestMatch = validValue;
                    bestDistance = distance;

                    if (distance == 0)
                    {
                        break;
                    }
                }
            }

            return match;
        }

        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        /// <param name="s">The first of the two strings.</param>
        /// <param name="t">The second of the two strings.</param>
        /// <returns>The Levenshtein cost.</returns>
        /// Source: http://rosettacode.org/wiki/Levenshtein_distance#C.23
        static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            for (int j = 1; j <= m; j++)
                for (int i = 1; i <= n; i++)
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