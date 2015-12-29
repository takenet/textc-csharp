using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Takenet.Text.Types;

namespace Takenet.Text.Processors
{
    /// <summary>
    /// Utility methods for handling types.
    /// </summary>
    public static class TypeUtil
    {
        public static bool IsNullable(Type type)
        {
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        // http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck, out Type baseType)
        {
            baseType = null;

            while (toCheck != null && toCheck != typeof (object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    baseType = toCheck;

                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static object[] GetParametersFromExpression(Expression expression, ParameterInfo[] methodParameters,
            bool allowNullOnNullableParameters)
        {
            var parameterArray = new object[methodParameters.Length];

            for (var i = 0; i < methodParameters.Length; i++)
            {
                var methodParameter = methodParameters[i];

                var parameterToken = expression.Tokens.FirstOrDefault(t => t != null &&
                                                                           t.Type.Name == methodParameter.Name);

                if (parameterToken != null)
                {
                    parameterArray[i] = parameterToken.Value;
                }
                else if (methodParameter.ParameterType == typeof (IRequestContext))
                {
                    parameterArray[i] = expression.Context;
                }
                else if (methodParameter.ParameterType == typeof (Expression))
                {
                    parameterArray[i] = expression;
                }
                else if (allowNullOnNullableParameters && IsNullable(methodParameter.ParameterType))
                {
                    // O parâmetro aceita de forma explicita o valor nulo
                    parameterArray[i] = null;
                }
                else
                {
                    throw new Exception(
                        string.Format("Could not find a token for mandatory parameter '{0}' on expression",
                            methodParameter.Name));
                }
            }
            return parameterArray;
        }

        public static void CheckSyntaxesForParameters(Syntax[] syntaxes, ParameterInfo[] methodParameters)
        {
            foreach (var syntax in syntaxes)
            {
                // Checa se a sintaxe cobre todos os parametros da ação
                foreach (var methodParameter in methodParameters)
                {
                    var tokenType = syntax.TokenTypes.FirstOrDefault(t => t.Name == methodParameter.Name);

                    if (tokenType != null)
                    {
                        var tokenTypeType = tokenType.GetType();

                        Type genericTokenType = null;

                        // Verifica se o tipo do parâmetro é compatível
                        if (TryGetGenericTokenTypeParameterType(tokenTypeType, out genericTokenType))
                        {
                            if (genericTokenType != methodParameter.ParameterType)
                            {
                                // Check if is a Nullable<T>
                                Type baseType;

                                if (
                                    !(IsSubclassOfRawGeneric(typeof (Nullable<>), methodParameter.ParameterType,
                                        out baseType) &&
                                      tokenType.IsOptional &&
                                      methodParameter.ParameterType.GetGenericArguments().FirstOrDefault() ==
                                      genericTokenType))
                                {
                                    throw new ArgumentException(
                                        string.Format(
                                            "Method parameter '{0}' type is incorrect in one or more syntaxes. Expected type is '{1}' and actual '{2}'.",
                                            methodParameter.Name, methodParameter.ParameterType.Name,
                                            genericTokenType.Name));
                                }
                            }
                        }
                    }
                    else if (methodParameter.ParameterType != typeof (IRequestContext) &&
                             methodParameter.ParameterType != typeof (Expression))
                    {
                        throw new ArgumentException(
                            string.Format("Method parameter '{0}' is not covered by one or more syntaxes",
                                methodParameter.Name));
                    }
                }
            }
        }

        public static bool TryGetGenericTokenTypeParameterType(Type tokenTypeType, out Type genericParameterType)
        {
            var result = false;
            genericParameterType = null;

            Type baseType = null;

            if (IsSubclassOfRawGeneric(typeof (TokenType<>), tokenTypeType, out baseType))
            {
                genericParameterType = baseType.GetGenericArguments().First();
                result = true;
            }

            return result;
        }

        public static Type GetGenericTokenTypeParameterType(Type tokenTypeType)
        {
            Type genericParameterType;
            TryGetGenericTokenTypeParameterType(tokenTypeType, out genericParameterType);
            return genericParameterType;
        }

        public static bool TryConvert(object value, Type conversionType, out object convertedValue)
        {
            // TODO: Move this to TypeUtil on SmartText
            try
            {
                try
                {
                    // Try using TypeDescriptor
                    convertedValue = TypeDescriptor
                        .GetConverter(conversionType)
                        .ConvertFrom(value);
                    return true;
                }
                catch (NotSupportedException)
                {
                    try
                    {
                        // Try again with Convert
                        convertedValue = Convert.ChangeType(value, conversionType);
                        return true;
                    }
                    catch (InvalidCastException)
                    {
                        Type baseType;
                        if (IsSubclassOfRawGeneric(typeof (Nullable<>), conversionType, out baseType))
                        {
                            var actualConversionType = conversionType.GetGenericArguments().FirstOrDefault();
                            if (actualConversionType != null &&
                                TryConvert(value, actualConversionType, out convertedValue))
                            {
                                return true;
                            }
                        }

                        throw;
                    }
                }
            }
            catch
            {
                convertedValue = null;
                return false;
            }
        }
    }
}