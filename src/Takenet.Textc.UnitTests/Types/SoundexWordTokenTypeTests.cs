using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Takenet.Textc.Types;

namespace Takenet.Textc.UnitTests.Types
{
    [TestFixture]
    public class SoundexWordTokenTypeTests
    {
        private static readonly Fixture _fixture = new Fixture();

        private string _name = _fixture.Create<string>();
        private bool _isContextual = false;
        private bool _isOptional = false;
        private bool _invertParsing = false;
        private IRequestContext _context = new RequestContext();

        private SoundexWordTokenType GetTarget()
        {
            return new SoundexWordTokenType(_name, _isContextual, _isOptional, _invertParsing);
        }

        private ITextCursor GetCursor(string value)
        {
            return new TextCursor(value, _context);
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
            var word = "right";
            var approximateWord = "wrong";
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
