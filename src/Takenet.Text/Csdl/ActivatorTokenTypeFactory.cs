using System;
using Takenet.Text.Types;

namespace Takenet.Text.Csdl
{
    public class ActivatorTokenTypeFactory : ITokenTypeFactory
    {
        public ITokenType Create(Type tokenType, string name, bool isContextual, bool isOptional,
            bool invertParsing)
        {
            return
                (ITokenType)
                    Activator.CreateInstance(tokenType, name, isContextual, isOptional, invertParsing);
        }
    }
}