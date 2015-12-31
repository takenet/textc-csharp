using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.PreProcessors
{
    public class TextNormalizerPreprocessor : ITextPreprocessor
    {
        private static readonly Regex SpecialCharsInNumberExpression = new Regex(@"((?<=\d)[^\w ]+(?=\d))", RegexOptions.Compiled);
        private static readonly Regex SpecialCharsExpression = new Regex(@"[^\w]+", RegexOptions.Compiled);
        private static readonly Regex MultipleSpacesExpression = new Regex(@"\s+", RegexOptions.Compiled);

        public TextNormalizerPreprocessor()
            : this(0)
        {
        }

        public TextNormalizerPreprocessor(int priority)
        {
            Priority = priority;
        }

        public Task<string> ProcessTextAsync(string text, IRequestContext context, CancellationToken cancellationToken)
        {
            var normalizedText = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < normalizedText.Length; i++)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(normalizedText[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(normalizedText[i]);
                }
            }
            normalizedText = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            normalizedText = SpecialCharsInNumberExpression.Replace(normalizedText, string.Empty);
                //remove any special chars between numbers
            normalizedText = SpecialCharsExpression.Replace(normalizedText, " ");
            normalizedText = MultipleSpacesExpression.Replace(normalizedText, " ");
            return Task.FromResult(normalizedText);
        }

        public int Priority { get; }
    }
}