using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Takenet.Textc.Metadata;
using Takenet.Textc.Types;

namespace Takenet.Textc.Csdl
{
    /// <summary>
    /// Represents a CSDL token.
    /// </summary>
    public class CsdlToken
    {
        public const string TOKEN_PATTERN =
            @"(?<name>[A-Za-z0-9]*?\+?):(?<type>[A-Za-z0-9]+\??~?)(?<initializer>(\(((\{.*?\})|('.*?')|(.*?))?\))?)";

        public const char NAME_TYPE_SEPARATOR = ':';
        public const char INITIALIZATOR_START = '(';
        public const char INITIALIZATOR_END = ')';
        public const char OPTIONAL_TOKEN = '?';
        public const char INVERT_PARSING_TOKEN = '~';
        public const char CONTEXTUAL_TOKEN = '+';
        public const char JSON_TOKEN_TYPE_PROPERTY_START = '{';
        public const char JSON_TOKEN_TYPE_PROPERTY_END = '}';
        public const string DEFAULT_TOKEN_TYPE_PROPERTY = "@DefaultTokenTypeProperty";
        public const char DEFAULT_TOKEN_TYPE_PROPERTY_DELIMITER = '\'';


        private static readonly ITokenTypeFactory TokenTypeFactory = new ActivatorTokenTypeFactory();

        public ITokenType ToTokenType(IDictionary<string, Type> tokenTypeTypeDictionary)
        {
            Type tokenTypeType;

            // Checks if the token type is registered
            if (tokenTypeTypeDictionary.TryGetValue(TokenTypeName, out tokenTypeType))
            {
                var tokenType = TokenTypeFactory.Create(tokenTypeType, Name, IsContextual, IsOptional, InvertParsing);

                // Initialize its properties
                if (TokenPropertiesDictionary != null)
                {
                    foreach (var propertyName in TokenPropertiesDictionary.Keys)
                    {
                        PropertyInfo property;
                        var isDefaultProperty = false;

                        if (propertyName.Equals(DEFAULT_TOKEN_TYPE_PROPERTY))
                        {
                            var defaultProperty = tokenTypeType
                                .GetProperties()
                                .Where(p =>
                                {
                                    var propertyAttribute =
                                        Attribute.GetCustomAttribute(p, typeof (TokenTypePropertyAttribute)) as
                                            TokenTypePropertyAttribute;
                                    return propertyAttribute != null && propertyAttribute.IsDefault;
                                })
                                .FirstOrDefault();

                            if (defaultProperty != null)
                            {
                                property = defaultProperty;
                                isDefaultProperty = true;
                            }
                            else
                            {
                                throw new ArgumentException(
                                    $"There's no default property on token type '{TokenTypeName}'");
                            }
                        }
                        else
                        {
                            property = tokenTypeType.GetProperty(propertyName);
                        }

                        if (property != null)
                        {
                            object propertyValue;
                            if (typeof (IConvertible).IsAssignableFrom(property.PropertyType))
                            {
                                propertyValue = Convert.ChangeType(TokenPropertiesDictionary[propertyName],
                                    property.PropertyType);
                            }
                            else if (isDefaultProperty &&
                                     property.PropertyType.IsArray &&
                                     typeof (IConvertible).IsAssignableFrom(property.PropertyType.GetElementType()))
                            {
                                var elementType = property.PropertyType.GetElementType();

                                var list = new ArrayList();
                                TokenPropertiesDictionary[propertyName]
                                    .Split(',')
                                    .Select(t => Convert.ChangeType(t, elementType))
                                    .ToList()
                                    .ForEach(e => list.Add(e));

                                propertyValue = list.ToArray(elementType);
                            }
                            else
                            {
                                propertyValue = JsonConvert.DeserializeObject(TokenPropertiesDictionary[propertyName],
                                    property.PropertyType);
                            }

                            property.SetValue(tokenType, propertyValue, null);
                        }
                        else
                        {
                            throw new ArgumentException(
                                $"There's no '{propertyName}' property on '{tokenTypeType.Name}' token type");
                        }
                    }
                }

                return tokenType;
            }

            throw new ArgumentException(
                $"Could not find token type '{TokenTypeName}' in the registered collection", nameof(tokenTypeTypeDictionary));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return TokenText;
        }

        /// <summary>
        /// Gets the tokens from pattern.
        /// </summary>
        /// <param name="syntaxPattern">The syntax pattern.</param>
        /// <returns></returns>
        public static CsdlToken[] GetTokensFromPattern(string syntaxPattern)
        {
            var matches = Regex.Matches(syntaxPattern, TOKEN_PATTERN, RegexOptions.ExplicitCapture);
            var tokens = new CsdlToken[matches.Count];

            for (var i = 0; i < matches.Count; i++)
            {
                tokens[i] = new CsdlToken(matches[i].Value, $"t{i}");
            }

            return tokens;
        }

        public CsdlToken(string tokenText)
            : this(tokenText, Guid.NewGuid().ToString())
        {
        }

        public CsdlToken(string tokenText, string alternativeName)
        {
            if (string.IsNullOrWhiteSpace(tokenText))
            {
                throw new ArgumentNullException(nameof(tokenText));
            }

            TokenText = tokenText;

            var separatorIndex = tokenText.IndexOf(NAME_TYPE_SEPARATOR);
            if (separatorIndex >= 0)
            {
                Name = tokenText.Substring(0, separatorIndex);

                if (string.IsNullOrEmpty(Name) ||
                    Name.Equals(CONTEXTUAL_TOKEN.ToString(CultureInfo.InvariantCulture),
                        StringComparison.OrdinalIgnoreCase))
                {
                    Name = alternativeName;
                }

                if (Name.EndsWith(CONTEXTUAL_TOKEN.ToString(CultureInfo.InvariantCulture)))
                {
                    IsContextual = true;
                    Name = Name.TrimEnd(CONTEXTUAL_TOKEN);
                }

                // Checks if the token has an initializator, that changes its instance properties.
                var tokenTypeInitializator = tokenText.Substring(separatorIndex + 1,
                    tokenText.Length - separatorIndex - 1);
                var initializatorStartIndex = tokenTypeInitializator.IndexOf(INITIALIZATOR_START);

                if (initializatorStartIndex >= 0)
                {
                    TokenTypeName = tokenTypeInitializator.Substring(0, initializatorStartIndex);
                    TokenPropertiesDictionary = new Dictionary<string, string>();

                    var initializatorEndIndex = tokenTypeInitializator.LastIndexOf(INITIALIZATOR_END);
                    var initializatorValues =
                        tokenTypeInitializator.Substring(initializatorStartIndex + 1,
                            initializatorEndIndex - initializatorStartIndex - 1).Trim();

                    // Check if the initialization content is a JSON
                    if (initializatorValues.Length > 0 &&
                        initializatorValues[0] == JSON_TOKEN_TYPE_PROPERTY_START &&
                        initializatorValues[initializatorValues.Length - 1] == JSON_TOKEN_TYPE_PROPERTY_END)
                    {
                        var jobject = JObject.Parse(initializatorValues);

                        foreach (var jtoken in jobject)
                        {
                            TokenPropertiesDictionary.Add(jtoken.Key, jtoken.Value.ToString());
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(initializatorValues))
                    {
                        TokenPropertiesDictionary.Add(DEFAULT_TOKEN_TYPE_PROPERTY,
                            initializatorValues.Trim(DEFAULT_TOKEN_TYPE_PROPERTY_DELIMITER));
                    }
                }
                else
                {
                    TokenTypeName = tokenTypeInitializator;
                }

                if (TokenTypeName.EndsWith(OPTIONAL_TOKEN.ToString(CultureInfo.InvariantCulture)))
                {
                    IsOptional = true;
                    TokenTypeName = TokenTypeName.TrimEnd(OPTIONAL_TOKEN);
                }

                if (TokenTypeName.EndsWith(INVERT_PARSING_TOKEN.ToString(CultureInfo.InvariantCulture)))
                {
                    InvertParsing = true;
                    TokenTypeName = TokenTypeName.TrimEnd(INVERT_PARSING_TOKEN);
                }
            }
            else
            {
                throw new ArgumentException($"Could not find the name type separator on token '{tokenText}'");
            }
        }

        internal CsdlToken(string name, string tokenTypeName, bool isOptional, bool invertParsing,
            IDictionary<string, string> tokenPropertiesDictionary)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(tokenTypeName))
            {
                throw new ArgumentNullException(nameof(tokenTypeName));
            }

            Name = name;
            TokenTypeName = tokenTypeName;
            IsOptional = isOptional;
            InvertParsing = invertParsing;
            TokenPropertiesDictionary = tokenPropertiesDictionary;
            TokenText = $"{name}:{tokenTypeName}";
        }

        public string TokenText { get; }

        public string Name { get; }

        public bool IsContextual { get; }

        public string TokenTypeName { get; }

        public IDictionary<string, string> TokenPropertiesDictionary { get; }

        public bool IsOptional { get; }

        public bool InvertParsing { get; }
    }
}