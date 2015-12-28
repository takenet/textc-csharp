using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Takenet.Text.Metadata;
using Takenet.Text.Templates;

namespace Takenet.Text.Csdl
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
        public const char JSON_TOKEN_TEMPLATE_PROPERTY_START = '{';
        public const char JSON_TOKEN_TEMPLATE_PROPERTY_END = '}';
        public const string DEFAULT_TOKEN_TEMPLATE_PROPERTY = "@DefaultTokenTemplateProperty";
        public const char DEFAULT_TOKEN_TEMPLATE_PROPERTY_DELIMITER = '\'';


        private static readonly ITokenTemplateFactory _tokenTemplateFactory = new ActivatorTokenTemplateFactory();

        public ITokenTemplate ToTokenTemplate(IDictionary<string, Type> tokenTemplateTypeDictionary)
        {
            Type tokenTemplateType;

            // Checks if the token template is registered
            if (tokenTemplateTypeDictionary.TryGetValue(TokenTemplateTypeName, out tokenTemplateType))
            {
                var tokenTemplate = _tokenTemplateFactory.Create(tokenTemplateType, Name, IsContextual, IsOptional,
                    InvertParsing);

                // Initialize its properties
                if (TokenPropertiesDictionary != null)
                {
                    foreach (var propertyName in TokenPropertiesDictionary.Keys)
                    {
                        PropertyInfo property;
                        var isDefaultProperty = false;

                        if (propertyName.Equals(DEFAULT_TOKEN_TEMPLATE_PROPERTY))
                        {
                            var defaultProperty = tokenTemplateType
                                .GetProperties()
                                .Where(p =>
                                {
                                    var propertyAttribute =
                                        Attribute.GetCustomAttribute(p, typeof (TokenTemplatePropertyAttribute)) as
                                            TokenTemplatePropertyAttribute;
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
                                    $"There's no default property on token template '{TokenTemplateTypeName}'");
                            }
                        }
                        else
                        {
                            property = tokenTemplateType.GetProperty(propertyName);
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

                            property.SetValue(tokenTemplate, propertyValue, null);
                        }
                        else
                        {
                            throw new ArgumentException(
                                $"There's no '{propertyName}' property on '{tokenTemplateType.Name}' token template type");
                        }
                    }
                }

                return tokenTemplate;
            }

            throw new ArgumentException(
                $"Could not find token template type '{TokenTemplateTypeName}' in the registered collection", nameof(tokenTemplateTypeDictionary));
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
                    TokenTemplateTypeName = tokenTypeInitializator.Substring(0, initializatorStartIndex);
                    TokenPropertiesDictionary = new Dictionary<string, string>();

                    var initializatorEndIndex = tokenTypeInitializator.LastIndexOf(INITIALIZATOR_END);
                    var initializatorValues =
                        tokenTypeInitializator.Substring(initializatorStartIndex + 1,
                            initializatorEndIndex - initializatorStartIndex - 1).Trim();

                    // Check if the initialization content is a JSON
                    if (initializatorValues.Length > 0 &&
                        initializatorValues[0] == JSON_TOKEN_TEMPLATE_PROPERTY_START &&
                        initializatorValues[initializatorValues.Length - 1] == JSON_TOKEN_TEMPLATE_PROPERTY_END)
                    {
                        var jobject = JObject.Parse(initializatorValues);

                        foreach (var jtoken in jobject)
                        {
                            TokenPropertiesDictionary.Add(jtoken.Key, jtoken.Value.ToString());
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(initializatorValues))
                    {
                        TokenPropertiesDictionary.Add(DEFAULT_TOKEN_TEMPLATE_PROPERTY,
                            initializatorValues.Trim(DEFAULT_TOKEN_TEMPLATE_PROPERTY_DELIMITER));
                    }
                }
                else
                {
                    TokenTemplateTypeName = tokenTypeInitializator;
                }

                if (TokenTemplateTypeName.EndsWith(OPTIONAL_TOKEN.ToString(CultureInfo.InvariantCulture)))
                {
                    IsOptional = true;
                    TokenTemplateTypeName = TokenTemplateTypeName.TrimEnd(OPTIONAL_TOKEN);
                }

                if (TokenTemplateTypeName.EndsWith(INVERT_PARSING_TOKEN.ToString(CultureInfo.InvariantCulture)))
                {
                    InvertParsing = true;
                    TokenTemplateTypeName = TokenTemplateTypeName.TrimEnd(INVERT_PARSING_TOKEN);
                }
            }
            else
            {
                throw new ArgumentException($"Could not find the name type separator on token '{tokenText}'");
            }
        }

        internal CsdlToken(string name, string tokenTemplateTypeName, bool isOptional, bool invertParsing,
            IDictionary<string, string> tokenPropertiesDictionary)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(tokenTemplateTypeName))
            {
                throw new ArgumentNullException(nameof(tokenTemplateTypeName));
            }

            Name = name;
            TokenTemplateTypeName = tokenTemplateTypeName;
            IsOptional = isOptional;
            InvertParsing = invertParsing;
            TokenPropertiesDictionary = tokenPropertiesDictionary;
            TokenText = $"{name}:{tokenTemplateTypeName}";
        }

        public string TokenText { get; }

        public string Name { get; }

        public bool IsContextual { get; }

        public string TokenTemplateTypeName { get; }

        public IDictionary<string, string> TokenPropertiesDictionary { get; }

        public bool IsOptional { get; }

        public bool InvertParsing { get; }
    }
}