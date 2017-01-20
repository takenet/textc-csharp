using NFluent;
using NUnit.Framework;
using Takenet.Textc.Types;

namespace Takenet.Textc.UnitTests.Types
{
    [TestFixture]
    public class TextTokenTypeTests : TokenTypeTestsBase<TextTokenType>
    {
        protected override TextTokenType GetTarget()
        {
            return new TextTokenType(Name, IsContextual, IsOptional, InvertParsing);
        }

        [Test]
        public void TryParseAValidTextShouldReturnTrue()
        {
            // Arrange
            var target = GetTarget();
            var text1 = "this other sample";
            var text2 = "this is a sample";

            target.ValidValues = new[] { text1, text2 };
            var cursor = GetCursor(text2);

            // Act
            string actual;
            var result = target.TryGetTokenFromInput(cursor, out actual);

            // Assert
            Check.That(result).IsTrue();
            Check.That(actual).Equals(text2);
            Check.That(cursor.IsEmpty).IsTrue();
        }

        [Test]
        public void TryParseAValidTextWithPartialValueShouldReturnTrue()
        {
            // Arrange
            var target = GetTarget();
            var text1 = "this other sample";
            var text2 = "this is a sample";
            var text = text2 + " with valid value";

            target.ValidValues = new[] { text1, text2 };
            var cursor = GetCursor(text);

            // Act
            string actual;
            var result = target.TryGetTokenFromInput(cursor, out actual);

            // Assert
            Check.That(result).IsTrue();
            Check.That(actual).Equals(text2);
            Check.That(cursor.IsEmpty).IsFalse();
            Check.That(cursor.All()).Equals("with valid value");
        }


        [Test]
        public void TryParseWithoutAValidValueShouldReturnTrue()
        {
            // Arrange
            var target = GetTarget();
            var text = "this is a sample";
            var cursor = GetCursor(text);

            // Act
            string actual;
            var result = target.TryGetTokenFromInput(cursor, out actual);

            // Assert
            Check.That(result).IsTrue();
            Check.That(actual).Equals(text);
            Check.That(cursor.IsEmpty).IsTrue();
        }
    }
}