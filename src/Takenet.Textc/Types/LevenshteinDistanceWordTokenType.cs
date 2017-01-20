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
                var distance = value.CalculateLevenshteinDistance(validValue);

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
    }
}