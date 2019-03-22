using System;
using NFluent;
using NUnit.Framework;
using AutoFixture;

namespace Takenet.Textc.UnitTests
{
    [TestFixture]
    public class RequestContextTests
    {
        private static readonly Fixture _fixture = new Fixture();

        [Test]
        public void SetAVariableShouldStoreTheValue()
        {
            // Arrange
            var requestContext = _fixture.Create<RequestContext>();
            var name = _fixture.Create<string>();
            var value = _fixture.Create<object>();

            // Act
            requestContext.SetVariable(name, value);

            // Assert
            Check.That(requestContext.GetVariable(name)).IsEqualTo(value);
        }

        [Test]
        public void SetAVariableWithNullShouldNotThrowAnException()
        {
            // Arrange
            var requestContext = _fixture.Create<RequestContext>();
            var name = _fixture.Create<string>();

            // Act
            requestContext.SetVariable(name, null);

            // Assert
            Check.That(requestContext.GetVariable(name)).IsNull();
        }

        [Test]
        public void SetAVariableWithNullNameShouldThrowArgumentNullException()
        {
            // Arrange            
            var requestContext = _fixture.Create<RequestContext>();            
            var value = _fixture.Create<object>();


            // Act and Assert
            Assert.Throws<ArgumentNullException>(() =>
                requestContext.SetVariable(null, value));
        }

        [Test]
        public void RemoveExistingVariableShouldRemoveTheValue()
        {
            // Arrange            
            var requestContext = _fixture.Create<RequestContext>();
            var name = _fixture.Create<string>();
            var value = _fixture.Create<object>();

            requestContext.SetVariable(name, value);

            // Act
            requestContext.RemoveVariable(name);

            // Assert
            Check.That(requestContext.GetVariable(name)).IsNull();            
        }

        [Test]
        public void RemoveANonExistingVariableShouldNotThrowAnException()
        {
            // Arrange
            var requestContext = _fixture.Create<RequestContext>();
            var name = _fixture.Create<string>();            

            // Act
            requestContext.RemoveVariable(name);
        }

        [Test]
        public void ClearExistingVariablesShouldRemoveTheValues()
        {
            // Arrange
            var requestContext = _fixture.Create<RequestContext>();
            var names = _fixture.Create<string[]>();
            var values = _fixture.Create<object[]>();

            for (int i = 0; i < names.Length; i++)
            {
                requestContext.SetVariable(names[i], values[i]);
            }

            // Act
            requestContext.Clear();

            // Assert
            foreach (var name in names)
            {
                Check.That(requestContext.GetVariable(name)).IsNull();
            }
        }
    }
}
