using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    [TokenType(ShortName = "SWord")]
    public class SoundexWordTokenType  : ValueTokenTypeBase<string>
    {
        public SoundexWordTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {

        }

        protected override bool HasMatch(string value, IRequestContext context, out string bestMatch)
        {
            var match = false;
            bestMatch = null;

            var valueSoundex = Soundex(value);
            foreach (var validValue in GetValidValues(context))
            {
                var candidateSoundex = Soundex(validValue);
                if (candidateSoundex.Equals(valueSoundex))
                {
                    match = true;
                    bestMatch = validValue;
                    break;
                }
            }

            return match;
        }

        /// <summary>
        /// Gets the soundex code for the specified word.
        /// Source: http://rosettacode.org/wiki/Soundex#C.23
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        private static string Soundex(string word)
        {
            const string soundexAlphabet = "0123012#02245501262301#202";
            var soundexString = "";
            var lastSoundexChar = '?';
            word = word.ToUpper();

            foreach (var c in from ch in word
                              where ch >= 'A' &&
                                    ch <= 'Z' &&
                                    soundexString.Length < 4
                              select ch)
            {
                char thisSoundexChar = soundexAlphabet[c - 'A'];

                if (soundexString.Length == 0)
                    soundexString += c;
                else if (thisSoundexChar == '#')
                    continue;
                else if (thisSoundexChar != '0' &&
                         thisSoundexChar != lastSoundexChar)
                    soundexString += thisSoundexChar;

                lastSoundexChar = thisSoundexChar;
            }

            return soundexString.PadRight(4, '0');
        }


    }
}
