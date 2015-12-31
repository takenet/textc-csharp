using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.PreProcessors
{
    public class DictionaryReplacerPreprocessor : ITextPreprocessor
    {
        public IDictionary<Regex, string> ReplacementDictionary { get; }

        public DictionaryReplacerPreprocessor(IDictionary<string, string> replacementDictionary)
            : this(replacementDictionary, 0)
        {
        }

        public DictionaryReplacerPreprocessor(IDictionary<string, string> replacementDictionary, int priority)
        {
            Priority = priority;
            if (replacementDictionary == null)
            {
                throw new ArgumentNullException(nameof(replacementDictionary));
            }

            ReplacementDictionary = new Dictionary<Regex, string>();

            foreach (var key in replacementDictionary.Keys)
            {
                ReplacementDictionary.Add(new Regex(key, RegexOptions.Compiled | RegexOptions.CultureInvariant),
                    replacementDictionary[key]);
            }
        }

        public Task<string> ProcessTextAsync(string text, IRequestContext context, CancellationToken cancellationToken)
        {
            var parsedText = text;

            foreach (var key in ReplacementDictionary.Keys)
            {
                parsedText = key.Replace(parsedText, ReplacementDictionary[key]);
            }

            return Task.FromResult(parsedText);
        }

        public int Priority { get; }
    }
}