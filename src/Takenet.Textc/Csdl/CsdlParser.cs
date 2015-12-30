using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Takenet.Textc.Metadata;
using Takenet.Textc.Processors;
using Takenet.Textc.Types;

namespace Takenet.Textc.Csdl
{
    /// <summary>
    /// Implements a parser for the Command Syntax Definition Language.
    /// </summary>
    public static class CsdlParser
    {
        private static readonly IDictionary<string, Type> TokenTypeDictionary = new Dictionary<string, Type>();

        static CsdlParser()
        {
            var loadedAssemblies = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a => a.GetCustomAttributes(typeof (TokenTypeLibraryAttribute), false).Any());

            foreach (var assembly in loadedAssemblies)
            {
                LoadTokenTypesFromAssembly(assembly);
            }
        }

        private static void LoadTokenTypesFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                var tokenTypeAttribute =
                    type.GetCustomAttributes(typeof (TokenTypeAttribute), false).FirstOrDefault() as
                        TokenTypeAttribute;

                if (tokenTypeAttribute != null &&
                    typeof (ITokenType).IsAssignableFrom(type))
                {
                    var registerTokenTypeMethod = typeof (CsdlParser).GetMethod(nameof(RegisterTokenType));
                    var genericRegisterTokenTypeMethod = registerTokenTypeMethod.MakeGenericMethod(type);

                    genericRegisterTokenTypeMethod.Invoke(null, null);
                }
            }
        }

        /// <summary>
        /// Registers a token type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public static void RegisterTokenType<T>() where T : ITokenType
        {
            var tokenType = typeof (T);

            var tokenTypeAttribute =
                Attribute.GetCustomAttribute(tokenType, typeof (TokenTypeAttribute)) as
                    TokenTypeAttribute;

            if (tokenTypeAttribute != null)
            {
                if (!TokenTypeDictionary.ContainsKey(tokenTypeAttribute.ShortName))
                {
                    TokenTypeDictionary.Add(tokenTypeAttribute.ShortName, tokenType);
                }
                else
                {
                    throw new ArgumentException(
                        $"There's already a token type with name '{tokenTypeAttribute.ShortName}' registered");
                }
            }
            else
            {
                throw new ArgumentException(
                    $"Type '{tokenType.Name}' is not decorated with '{nameof(TokenTypeAttribute)}'");
            }
        }

        /// <summary>
        /// Unregisters a token type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public static void UnregisterTokenType<T>() where T : ITokenType
        {
            var tokenType = typeof (T);

            var tokenTypeAttribute =
                Attribute.GetCustomAttribute(tokenType, typeof (TokenTypeAttribute)) as
                    TokenTypeAttribute;

            if (tokenTypeAttribute != null)
            {
                if (TokenTypeDictionary.ContainsKey(tokenTypeAttribute.ShortName))
                {
                    TokenTypeDictionary.Remove(tokenTypeAttribute.ShortName);
                }
                else
                {
                    throw new ArgumentException(
                        $"There's no token type with name '{tokenTypeAttribute.ShortName}' registered");
                }
            }
            else
            {
                throw new ArgumentException(
                    $"Type '{tokenType.Name}' is not decorated with '{nameof(TokenTypeAttribute)}'");
            }
        }

        /// <summary>
        /// Parses a CSDL string into a instance of <see cref="Syntax" />.
        /// </summary>
        /// <param name="syntaxPattern">The syntax pattern.</param>
        /// <returns></returns>
        public static Syntax Parse(string syntaxPattern)
        {
            return Parse(syntaxPattern, CultureInfo.InvariantCulture);            
        }

        /// <summary>
        /// Parses a CSDL string into a instance of <see cref="Syntax" />.
        /// </summary>
        /// <param name="syntaxPattern">The syntax pattern.</param>
        /// <param name="culture">The syntax culture.</param>
        /// <returns></returns>
        public static Syntax Parse(string syntaxPattern, CultureInfo culture)
        {
            var syntax = new CsdlSyntax(syntaxPattern);
            return syntax.GetSyntax(TokenTypeDictionary, culture);
        }

        /// <summary>
        /// Creates a simple CSDL string for a method.
        /// </summary>
        public static CsdlSyntax CreateBasicSyntaxForMethod(MethodInfo methodInfo, bool rightToLeftParsing, bool perfectMatchOnly)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var csdlTokenList = new List<CsdlToken>();

            foreach (var parameter in methodInfo.GetParameters())
            {
                var tokenTypeType =
                    TokenTypeDictionary
                        .FirstOrDefault(v => TypeUtil.GetGenericTokenTypeParameterType(v.Value) == parameter.ParameterType);

                if (tokenTypeType.Key != null)
                {
                    var csdlToken = new CsdlToken(parameter.Name, tokenTypeType.Key,
                        TypeUtil.IsNullable(parameter.ParameterType), false, null);
                    csdlTokenList.Add(csdlToken);
                }
                else if (parameter.ParameterType != typeof (IRequestContext) &&
                         parameter.ParameterType != typeof (Expression))
                {
                    throw new ArgumentException(
                        $"There's no registered token type for type '{parameter.ParameterType}'");
                }
            }

            var csdlSyntax = new CsdlSyntax(rightToLeftParsing, perfectMatchOnly, csdlTokenList.ToArray());
            return csdlSyntax;
        }
    }
}