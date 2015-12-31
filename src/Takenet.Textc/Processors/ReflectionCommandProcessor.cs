using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.Processors
{
    /// <summary>
    /// Processes commands using type reflection.
    /// </summary>
    public class ReflectionCommandProcessor : ICommandProcessor
    {
        private readonly bool _allowNullOnNullableParameters;
        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _methodParameters;
        private readonly object _processor;

        public ReflectionCommandProcessor(object processor, string methodName, bool allowNullOnNullableParameters = true,
            IOutputProcessor outputProcessor = null, params Syntax[] syntaxes)
        {
            // Static validation
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            if (syntaxes == null || syntaxes.Length == 0)
            {
                throw new ArgumentNullException(nameof(syntaxes));
            }

            _processor = processor;
            _allowNullOnNullableParameters = allowNullOnNullableParameters;
            Syntaxes = syntaxes;

            // Dynamic validation
            var processorType = processor.GetType();
            _method = processorType.GetMethod(methodName);

            if (_method == null)
            {
                throw new ArgumentException($"Type '{processorType.Name}' doesn't contains method '{methodName}'", nameof(methodName));
            }

            if (!typeof(Task).IsAssignableFrom(_method.ReturnType))
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

        public Task ProcessAsync(Expression expression, CancellationToken cancellationToken)
        {
            var parameters = TypeUtil.GetParametersFromExpression(
                expression,
                _methodParameters,
                _allowNullOnNullableParameters,
                cancellationToken);

            Task commandOutputTask;

            try
            {
                commandOutputTask = (Task)_method.Invoke(
                    _processor,
                    parameters);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            return commandOutputTask;
        }
    }
}