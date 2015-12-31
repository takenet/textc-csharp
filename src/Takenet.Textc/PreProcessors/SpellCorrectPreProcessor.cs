using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.PreProcessors
{
    public class SpellCorrectPreprocessor : ITextPreprocessor
    {
        private static SpellCorrect _spellCorrect;

        public SpellCorrectPreprocessor(Uri sampleDataFilePath)
            : this(File.ReadAllText(sampleDataFilePath.LocalPath), 0)
        {
        }

        public SpellCorrectPreprocessor(Uri sampleDataFilePath, int priority)
            : this(File.ReadAllText(sampleDataFilePath.LocalPath), priority)
        {
        }

        public SpellCorrectPreprocessor(string sampleData)
            : this(sampleData, 0)
        {
        }

        public SpellCorrectPreprocessor(string sampleData, int priority)
        {
            var corpus = new Corpus(sampleData);
            _spellCorrect = new SpellCorrect(corpus);

            Priority = priority;
        }

        public SpellCorrectPreprocessor(IEnumerable<string> sampleData)
            : this(sampleData, 0)
        {
        }

        public SpellCorrectPreprocessor(IEnumerable<string> sampleData, int priority)
        {
            var corpus = new Corpus(sampleData);
            _spellCorrect = new SpellCorrect(corpus);

            Priority = priority;
        }

        public Task<string> ProcessTextAsync(string text, IRequestContext context, CancellationToken cancellationToken)
        {
            var words = text.Split(' ');

            var correctTextBuilder = new StringBuilder();

            foreach (var word in words)
            {
                decimal number = 0;
                string correctedWord = null;

                if (!decimal.TryParse(word, out number))
                {
                    if (word.Length > 1)
                    {
                        correctedWord = _spellCorrect.Correct(word);
                    }
                }

                if (correctedWord != null)
                {
                    correctTextBuilder.AppendFormat("{0} ", correctedWord);
                }
                else
                {
                    correctTextBuilder.AppendFormat("{0} ", word);
                }
            }

            return Task.FromResult(correctTextBuilder.ToString().Trim());
        }

        public int Priority { get; set; }
    }
}