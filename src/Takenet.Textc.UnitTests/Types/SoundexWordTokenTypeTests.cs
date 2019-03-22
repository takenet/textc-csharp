using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using AutoFixture;
using Takenet.Textc.Types;

namespace Takenet.Textc.UnitTests.Types
{
    [TestFixture]
    public class SoundexWordTokenTypeTests : TokenTypeTestsBase<SoundexWordTokenType>
    {
        protected override SoundexWordTokenType GetTarget()
        {
            return new SoundexWordTokenType(Name, IsContextual, IsOptional, InvertParsing);
        }

        [Test]
        public void TryParseAValidSoundexWordShouldReturnTrue()
        {
            // Arrange
            var target = GetTarget();
            var word = "ezampul";
            var approximateWord = "ezampul";
            target.ValidValues = new[] { word };
            var cursor = GetCursor(approximateWord);

            // Act
            string actual;
            var result = target.TryGetTokenFromInput(cursor, out actual);

            // Assert
            Check.That(result).IsTrue();
            Check.That(actual).Equals(word);
        }

        [Test]
        public void TryParseAInvalidSoundexWordShouldReturnFalse()
        {
            // Arrange
            var target = GetTarget();
            var word = "Catherine";
            var approximateWord = "Katherine";
            target.ValidValues = new[] { word };            
            var cursor = GetCursor(approximateWord);

            // Act
            string actual;
            var result = target.TryGetTokenFromInput(cursor, out actual);

            // Assert
            Check.That(result).IsFalse();
            Check.That(actual).IsNull();
        }
    }
}
