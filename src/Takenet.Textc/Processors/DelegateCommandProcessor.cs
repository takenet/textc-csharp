using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.Processors
{
    /// <summary>
    /// Processes commands using delegates.
    /// </summary>
    public class DelegateCommandProcessor : ICommandProcessor
    {
        private readonly Delegate _delegate;
        private readonly ParameterInfo[] _actionParameters;
        private readonly bool _allowNullOnNullableParameters;

        public DelegateCommandProcessor(Delegate @delegate, bool allowNullOnNullableParameters = true, IOutputProcessor outputProcessor = null, params Syntax[] syntaxes)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            if (!typeof(Task).IsAssignableFrom(@delegate.Method.ReturnType))
            {
                throw new ArgumentException("The delegate must return a Task", nameof(@delegate));
            }

            if (outputProcessor != null && !@delegate.Method.ReturnType.IsGenericType)
            {
                throw new ArgumentException("A delegate with return value is required to use with an output processor", nameof(outputProcessor));
            }

            if (syntaxes == null || syntaxes.Length == 0)
            {
                throw new ArgumentNullException(nameof(syntaxes));
            }            
           
            _delegate = @delegate;
            _allowNullOnNullableParameters = allowNullOnNullableParameters;
            OutputProcessor = outputProcessor;
            Syntaxes = syntaxes;

            _actionParameters = _delegate.Method.GetParameters();
            TypeUtil.CheckSyntaxesForParameters(syntaxes, _actionParameters);
        }

        public Syntax[] Syntaxes { get; }

        public IOutputProcessor OutputProcessor { get; }

        public Task ProcessAsync(Expression expression, CancellationToken cancellationToken)
        {
            var parameterArray = TypeUtil.GetParametersFromExpression(
                expression,
                _actionParameters,
                _allowNullOnNullableParameters,
                cancellationToken);

            Task commandOutputTask;

            try
            {
                commandOutputTask = (Task)_delegate.DynamicInvoke(parameterArray);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            return commandOutputTask;
        }

        public static DelegateCommandProcessor Create(Func<Task> func, IOutputProcessor outputProcessor,
            params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1>(Func<T1, Task> func, IOutputProcessor outputProcessor,
            params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2>(Func<T1, T2, Task> func, IOutputProcessor outputProcessor,
            params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3>(Func<T1, T2, T3, Task> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Task> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, Task> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }


        public static DelegateCommandProcessor Create<TResult>(Func<Task<TResult>> func, IOutputProcessor outputProcessor,
            params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, TResult>(Func<T1, Task<TResult>> func, IOutputProcessor outputProcessor,
            params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, TResult>(Func<T1, T2, Task<TResult>> func, IOutputProcessor outputProcessor,
           params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, TResult>(Func<T1, T2, T3, Task<TResult>> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, Task<TResult>> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, Task<TResult>> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, Task<TResult>> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return new DelegateCommandProcessor(func, true, outputProcessor, syntaxes);
        }
    }
}