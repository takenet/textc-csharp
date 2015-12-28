using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Takenet.Text.Processors
{
    public class ReflectionCommandProcessor : ICommandProcessor
    {
        private readonly bool _allowNullOnNullableParameters;
        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _methodParameters;
        private readonly object _processor;

        public ReflectionCommandProcessor(object processor, string methodName, bool allowNullOnNullableParameters,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            _processor = processor;
            var processorType = processor.GetType();

            if (syntaxes == null ||
                syntaxes.Length == 0)
            {
                throw new ArgumentNullException(nameof(syntaxes));
            }

            Syntaxes = syntaxes;

            _allowNullOnNullableParameters = allowNullOnNullableParameters;

            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            _method = processorType.GetMethod(methodName);

            if (_method == null)
            {
                throw new ArgumentException($"Type '{processorType.Name}' doesn't contains method '{methodName}'");
            }

            if (!typeof (Task).IsAssignableFrom(_method.ReturnType))
            {
                throw new ArgumentException("The method must return a Task");
            }

            if (outputProcessor != null &&
                !_method.ReturnType.IsGenericType)
            {
                throw new ArgumentException("A method with return value is required to use with an output processor");
            }

            OutputProcessor = outputProcessor;

            _methodParameters = _method.GetParameters();

            TypeUtil.CheckSyntaxesForParameters(syntaxes, _methodParameters);
        }

        public Syntax[] Syntaxes { get; }

        public IOutputProcessor OutputProcessor { get; }

        public Task ProcessAsync(Expression expression)
        {
            var parameters = TypeUtil.GetParametersFromExpression(
                expression,
                _methodParameters,
                _allowNullOnNullableParameters);

            Task commandOutput;

            try
            {
                commandOutput = (Task) _method.Invoke(
                    _processor,
                    parameters);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            return commandOutput;
        }
    }
}