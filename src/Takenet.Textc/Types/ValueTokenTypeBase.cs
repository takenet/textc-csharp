using System;
using System.Collections.Generic;
using System.Linq;
using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    public abstract class ValueTokenTypeBase<T> : TokenType<T>
    {
        protected ValueTokenTypeBase(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        [TokenTypeProperty(IsDefault = true)]
        public T[] ValidValues { get; internal set; }

        [TokenTypeProperty]
        public string ValidValuesVariableName { get; internal set; }

        [TokenTypeProperty]
        public string ContextVariableName { get; internal set; }

        public override bool TryGetTokenFromInput(ITextCursor textCursor, out T token)
        {
            var match = false;
            token = default(T);

            var word = textCursor.Next();
            T parsedWord;

            if (!string.IsNullOrEmpty(word) &&
                TryParseWord(word, out parsedWord))
            {
                var validValues = GetValidValues(textCursor.Context);
                if (validValues != null)
                {
                    match = HasMatch(parsedWord, textCursor.Context, out token);
                }
                else
                {
                    token = parsedWord;
                    match = true;
                }
            }

            return match;
        }

        public override bool TryGetTokenFromContext(IRequestContext context, out T token)
        {
            var match = false;
            token = default(T);

            var variableName = Name;
            if (!string.IsNullOrWhiteSpace(ContextVariableName))
            {
                variableName = ContextVariableName;
            }

            var objTokenValue = context.GetVariable(variableName);
            if (objTokenValue != null)
            {
                var tokenValue = (T)objTokenValue;
                var validValues = GetValidValues(context);

                if (validValues != null)
                {
                    match = HasMatch(tokenValue, context, out token);
                }
                else
                {
                    token = tokenValue;
                    match = true;
                }
            }

            return match;
        }

        protected virtual bool TryParseWord(string word, out T value)
        {
            try
            {
                value = (T)Convert.ChangeType(word, typeof(T));
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }

        protected virtual bool Compare(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        protected virtual bool HasMatch(T value, IRequestContext context, out T bestMatch)
        {
            var hasMatch = false;
            bestMatch = default(T);

            var validValues = GetValidValues(context);

            if (validValues.Any(v => Compare(v, value)))
            {
                bestMatch = validValues.First(v => Compare(v, value));
                hasMatch = true;
            }

            return hasMatch;
        }

        protected T[] GetValidValues(IRequestContext context)
        {
            if (!string.IsNullOrWhiteSpace(ValidValuesVariableName))
            {
                return context.GetVariable(ValidValuesVariableName) as T[] ?? new T[0];
            }
            return ValidValues;
        }
    }
}