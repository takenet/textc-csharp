using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Takenet.Textc.PreProcessors
{
    /// <summary>
    /// http://www.codegrunt.co.uk/2010/11/02/C-Sharp-Norvig-Spelling-Corrector.html
    /// </summary>
    internal class SpellCorrect
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        private readonly ICorpus corpus;

        public SpellCorrect(ICorpus corpus)
        {
            this.corpus = corpus;
        }

        private IEnumerable<string> Edits(string word)
        {
            var splits = from i in Enumerable.Range(0, word.Length)
                select new {a = word.To(i), b = word.From(i)};
            var deletes = from s in splits
                where s.b != ""
                // Guaranteed not null
                select s.a + s.b.From(1);
            var transposes = from s in splits
                where s.b.Length > 1
                select s.a + s.b[1] + s.b[0] + s.b.From(2);
            var replaces = from s in splits
                from c in Alphabet
                select s.a + c + s.b.From(1);
            var inserts = from s in splits
                from c in Alphabet
                select s.a + c + s.b;

            return deletes
                .Union(transposes)
                .Union(replaces)
                .Union(inserts);
        }

        public IEnumerable<string> Corrections(string word)
        {
            if (corpus.Contains(word)) return new[] {word};

            var edits = Edits(word);

            var knownEdits = corpus.Known(edits);

            if (knownEdits.Any())
            {
                return knownEdits
                    .OrderByDescending(corpus.Rank);
            }

            var secondPass = from e1 in edits
                from e2 in Edits(e1)
                where corpus.Contains(e2)
                select e2;

            return secondPass.Any() ? secondPass.OrderByDescending(corpus.Rank) : null;
            //return secondPass.Any() ? secondPass : new[] { word };
        }

        public string Correct(string word)
        {
            string wordCorrection = null;

            var corrections = Corrections(word);

            if (corrections != null)
            {
                wordCorrection = corrections
                    .First();
            }

            return wordCorrection;
        }

        //}

        //    return corrections.First();
        //    var corrections = Corrections(word).OrderByDescending(corpus.Rank);
        //{

        //public string Correct(string word)
    }

    internal interface ICorpus
    {
        int Rank(string word);
        bool Contains(string word);
        IEnumerable<string> Known(IEnumerable<string> words);
    }

    internal class Corpus : ICorpus
    {
        private readonly Dictionary<string, int> rankings;

        public Corpus(string sample) : this(ExtractWords(sample))
        {
        }

        public Corpus(IEnumerable<string> sample)
        {
            rankings = sample.Select(w => w.ToLower())
                .GroupBy(w => w)
                .ToDictionary(w => w.Key, w => w.Count());
        }

        public int Rank(string word)
        {
            int ret;
            return rankings.TryGetValue(word, out ret) ? ret : 1;
        }

        public bool Contains(string word)
        {
            return rankings.ContainsKey(word);
        }

        public IEnumerable<string> Known(IEnumerable<string> words)
        {
            return words.Where(Contains);
        }

        private static IEnumerable<string> ExtractWords(string str)
        {
            return Regex.Matches(str, "[a-z]+", RegexOptions.IgnoreCase)
                .Cast<Match>()
                .Select(m => m.Value);
        }
    }

    internal static class StringExtensions
    {
        public static string From(this string str, int n)
        {
            if (str == null) return null;

            var len = str.Length;

            if (n >= len) return "";
            if (n == 0 || -n >= len) return str;

            return str.Substring((len + n)%len, (len - n)%len);
        }

        public static string To(this string str, int n)
        {
            if (str == null) return null;

            var len = str.Length;

            if (n == 0 || -n >= len) return "";
            if (n >= len) return str;

            return str.Substring(0, (len + n)%len);
        }
    }
}