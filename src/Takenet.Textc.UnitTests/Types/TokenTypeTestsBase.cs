using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Takenet.Textc.Types;

namespace Takenet.Textc.UnitTests.Types
{
    public abstract class TokenTypeTestsBase<T> where T : ITokenType
    {
        protected static readonly Fixture Fixture = new Fixture();

        protected string Name = Fixture.Create<string>();
        protected bool IsContextual = false;
        protected bool IsOptional = false;
        protected bool InvertParsing = false;
        protected IRequestContext Context = new RequestContext();        

        protected virtual ITextCursor GetCursor(string value)
        {
            return new TextCursor(value, Context);
        }

        protected abstract T GetTarget();
    }
}
