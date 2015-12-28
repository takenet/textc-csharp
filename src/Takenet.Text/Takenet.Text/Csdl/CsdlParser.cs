using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Takenet.Text.Metadata;
using Takenet.Text.Processors;
using Takenet.Text.Templates;

namespace Takenet.Text.Csdl
{
    /// <summary>
    /// Implements a parser for the Command Syntax Definition Language.
    /// </summary>
    public static class CsdlParser
    {
        private static readonly IDictionary<string, Type> TokenTemplateTypeDictionary = new Dictionary<string, Type>();

        static CsdlParser()
        {
            var loadedAssemblies = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a => a.GetCustomAttributes(typeof (TokenTemplateLibraryAttribute), false).Any());

            foreach (var assembly in loadedAssemblies)
            {
                LoadTemplatesFromAssembly(assembly);
            }
        }

        private static void LoadTemplatesFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                var tokenTemplateAttribute =
                    type.GetCustomAttributes(typeof (TokenTemplateAttribute), false).FirstOrDefault() as
                        TokenTemplateAttribute;

                if (tokenTemplateAttribute != null &&
                    typeof (ITokenTemplate).IsAssignableFrom(type))
                {
                    var registerTokenTemplateMethod = typeof (CsdlParser).GetMethod("RegisterTokenTemplate");
                    var genericRegisterTokenTemplateMethod = registerTokenTemplateMethod.MakeGenericMethod(type);

                    genericRegisterTokenTemplateMethod.Invoke(null, null);
                }
            }
        }

        /// <summary>
        /// Registers a token template type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public static void RegisterTokenTemplate<T>() where T : ITokenTemplate
        {
            var tokenTemplateType = typeof (T);

            var tokenTemplateAttribute =
                Attribute.GetCustomAttribute(tokenTemplateType, typeof (TokenTemplateAttribute)) as
                    TokenTemplateAttribute;

            if (tokenTemplateAttribute != null)
            {
                if (!TokenTemplateTypeDictionary.ContainsKey(tokenTemplateAttribute.ShortName))
                {
                    TokenTemplateTypeDictionary.Add(tokenTemplateAttribute.ShortName, tokenTemplateType);
                }
                else
                {
                    throw new ArgumentException(
                        $"There's already a token template with name '{tokenTemplateAttribute.ShortName}' registered");
                }
            }
            else
            {
                throw new ArgumentException(
                    $"Type '{tokenTemplateType.Name}' is not decorated with TokenTemplateAttribute");
            }
        }

        /// <summary>
        /// Unregisters a token template type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public static void UnregisterTokenTemplate<T>() where T : ITokenTemplate
        {
            var tokenTemplateType = typeof (T);

            var tokenTemplateAttribute =
                Attribute.GetCustomAttribute(tokenTemplateType, typeof (TokenTemplateAttribute)) as
                    TokenTemplateAttribute;

            if (tokenTemplateAttribute != null)
            {
                if (TokenTemplateTypeDictionary.ContainsKey(tokenTemplateAttribute.ShortName))
                {
                    TokenTemplateTypeDictionary.Remove(tokenTemplateAttribute.ShortName);
                }
                else
                {
                    throw new ArgumentException(
                        $"There's no token template with name '{tokenTemplateAttribute.ShortName}' registered");
                }
            }
            else
            {
                throw new ArgumentException(
                    $"Type '{tokenTemplateType.Name}' is not decorated with TokenTemplateAttribute");
            }
        }

        /// <summary>
        /// Parses a CSDL string into a instance of <see cref="Syntax" />.
        /// </summary>
        /// <param name="syntaxPattern">The syntax pattern.</param>
        /// <returns></returns>
        public static Syntax Parse(string syntaxPattern)
        {
            var syntax = new CsdlSyntax(syntaxPattern);
            return syntax.ToSyntax(TokenTemplateTypeDictionary);
        }

        /// <summary>
        /// Creates a simple CSDL string for a method.
        /// </summary>
        public static CsdlSyntax CreateBasicSyntaxForMethod(MethodInfo methodInfo, bool rightToLeftParsing,
            bool perfectMatchOnly)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var parameters = methodInfo.GetParameters();

            var csdlTokenList = new List<CsdlToken>();

            foreach (var parameter in methodInfo.GetParameters())
            {
                var tokenTemplateType =
                    TokenTemplateTypeDictionary
                        .FirstOrDefault(v => TypeUtil.GetGenericTokenTemplateParameterType(v.Value) == parameter.ParameterType);

                if (tokenTemplateType.Key != null)
                {
                    var csdlToken = new CsdlToken(parameter.Name, tokenTemplateType.Key,
                        TypeUtil.IsNullable(parameter.ParameterType), false, null);
                    csdlTokenList.Add(csdlToken);
                }
                else if (parameter.ParameterType != typeof (IRequestContext) &&
                         parameter.ParameterType != typeof (Expression))
                {
                    throw new ArgumentException(
                        $"There's no registered token template for type '{parameter.ParameterType}'");
                }
            }

            var csdlSyntax = new CsdlSyntax(rightToLeftParsing, perfectMatchOnly, csdlTokenList.ToArray());
            return csdlSyntax;
        }
    }
}