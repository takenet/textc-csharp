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
                var distance = ComputeLevenshteinDistance(value, validValue);

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
        private static int ComputeLevenshteinDistance(string s, string t)
        {
            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (var i = 1; i <= n; i++)
            {
                //Step 4
                for (var j = 1; j <= m; j++)
                {
                    // Step 5
                    var cost = t[j - 1] == s[i - 1] ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}