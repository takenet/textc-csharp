using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.Text.PreProcessors
{
    public class SpellCorrectPreProcessor : ITextPreProcessor
    {
        private static SpellCorrect _spellCorrect;

        public SpellCorrectPreProcessor(Uri sampleDataFilePath)
            : this(File.ReadAllText(sampleDataFilePath.LocalPath), 0)
        {
        }

        public SpellCorrectPreProcessor(Uri sampleDataFilePath, int priority)
            : this(File.ReadAllText(sampleDataFilePath.LocalPath), priority)
        {
        }

        public SpellCorrectPreProcessor(string sampleData)
            : this(sampleData, 0)
        {
        }

        public SpellCorrectPreProcessor(string sampleData, int priority)
        {
            var corpus = new Corpus(sampleData);
            _spellCorrect = new SpellCorrect(corpus);

            Priority = priority;
        }

        public SpellCorrectPreProcessor(IEnumerable<string> sampleData)
            : this(sampleData, 0)
        {
        }

        public SpellCorrectPreProcessor(IEnumerable<string> sampleData, int priority)
        {
            var corpus = new Corpus(sampleData);
            _spellCorrect = new SpellCorrect(corpus);

            Priority = priority;
        }

        public Task<string> ProcessTextAsync(string text, IRequestContext context)
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