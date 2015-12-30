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
    public class DoubleMetaphoneWordTokenTypeTests : TokenTypeTestsBase<DoubleMetaphoneWordTokenType>
    {
        protected override DoubleMetaphoneWordTokenType GetTarget()
        {
            return new DoubleMetaphoneWordTokenType(Name, IsContextual, IsOptional, InvertParsing);
        }

        [Test]
        public void TryParseAValidMetaphoneWordShouldReturnTrue()
        {
            // Arrange
            var target = GetTarget();
            var word = "Johnathan";
            var approximateWord = "Jonathan";
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
        public void TryParseAInvalidMetaphoneWordShouldReturnFalse()
        {
            // Arrange
            var target = GetTarget();
            var word = "John";
            var approximateWord = "Smith";
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
