namespace Takenet.Textc.Types
{
    /// <summary>
    /// Base class for token types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TokenType<T> : ITokenType
    {
        protected TokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
        {
            Name = name;
            IsContextual = isContextual;
            IsOptional = isOptional;
            InvertParsing = invertParsing;
        }

        public override string ToString()
        {
            return $"{Name}:{GetType().Name}";
        }

        public abstract bool TryGetTokenFromInput(ITextCursor textCursor, out T token);

        public abstract bool TryGetTokenFromContext(IRequestContext context, out T token);

        public string Name { get; }

        public bool IsContextual { get; }

        public bool IsOptional { get; }

        public bool InvertParsing { get; }

        public bool TryGetTokenFromInput(ITextCursor textCursor, out object token)
        {
            var result = false;
            var genericToken = default(T);

            if (!textCursor.IsEmpty)
            {
                result = TryGetTokenFromInput(textCursor, out genericToken);
            }

            if (result)
            {
                token = genericToken;
            }
            else
            {
                token = null;
            }

            return result;
        }

        public bool TryGetTokenFromContext(IRequestContext context, out object token)
        {
            T genericToken;
            var result = TryGetTokenFromContext(context, out genericToken);

            if (result)
            {
                token = genericToken;
            }
            else
            {
                token = null;
            }

            return result;
        }
    }
}