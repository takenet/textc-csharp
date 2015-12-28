using System;
using NFluent;
using Ploeh.AutoFixture.Xunit2;
using Takenet.Text;
using Xunit;

namespace Takenet.SmartSms.UnitTests
{
    public class RequestContextTests
    {
        [Fact]
        public void Success()
        {
            // Only for NCrunch recognize the class
            // PS: You must use StaticAnalysis in NCrunch solution configuration.
        }

        [Theory, AutoData]
        public void SetVariable_ValidVariableValue_StoresValue(RequestContext requestContext, string name, object value)
        {
            // Act
            requestContext.SetVariable(name, value);

            // Assert
            Check.That(requestContext.GetVariable(name)).IsEqualTo(value);
        }

        [Theory, AutoData]
        public void SetVariable_NullVariableValue_RemovesValue(RequestContext requestContext, string name)
        {
            // Act
            requestContext.SetVariable(name, null);

            // Assert
            Check.That(requestContext.GetVariable(name)).IsNull();
        }

        [Theory, AutoData]
        public void SetVariable_NullVariableName_ThrowsArgumentNullException(RequestContext requestContext, object value)
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() =>
                requestContext.SetVariable(null, value));
        }

        [Theory, AutoData]
        public void RemoveVariable_ExistingVariable_RemovesValue(RequestContext requestContext, string name, object value)
        {
            // Arrange
            requestContext.SetVariable(name, value);

            // Act
            requestContext.RemoveVariable(name);

            // Assert
            Check.That(requestContext.GetVariable(name)).IsNull();            
        }

        [Theory, AutoData]
        public void RemoveVariable_NonExistingVariable_DoNotThrowException(RequestContext requestContext, string name, object value)
        {
            // Act
            requestContext.RemoveVariable(name);
        }

        [Theory, AutoData]
        public void Clear_ExistingVariables_RemoveValues(RequestContext requestContext, string[] names, object[] values)
        {
            // Arrange
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
