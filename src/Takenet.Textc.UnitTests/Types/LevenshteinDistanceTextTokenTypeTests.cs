using NFluent;
using NUnit.Framework;
using Takenet.Textc.Types;

namespace Takenet.Textc.UnitTests.Types
{
    [TestFixture]
    public class LevenshteinDistanceTextTokenTypeTests : TokenTypeTestsBase<LevenshteinDistanceTextTokenType>
    {
        protected override LevenshteinDistanceTextTokenType GetTarget()
        {
            return new LevenshteinDistanceTextTokenType(Name, IsContextual, IsOptional, InvertParsing);
        }

        [Test]
        public void TryParseAValidLdTextShouldReturnTrue()
        {
            // Arrange
            var target = GetTarget();
            var text1 = "this is other sample";
            var text2 = "this is a sample";
            var approximateText = "this is an sample";
            target.ValidValues = new[] { text1, text2 };
            var cursor = GetCursor(approximateText);

            // Act
            string actual;
            var result = target.TryGetTokenFromInput(cursor, out actual);

            // Assert
            Check.That(result).IsTrue();
            Check.That(actual).Equals(text2);
        }

        [Test]
        public void TryParseAInvalidLdTextShouldReturnFalse()
        {
            // Arrange
            var target = GetTarget();
            var text = "this is a sample";
            var differentText = "this text is very different";
            target.ValidValues = new[] { text };
            var cursor = GetCursor(differentText);

            // Act
            string actual;
            var result = target.TryGetTokenFromInput(cursor, out actual);

            // Assert
            Check.That(result).IsFalse();
            Check.That(actual).IsNull();
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
        }
    }
}
