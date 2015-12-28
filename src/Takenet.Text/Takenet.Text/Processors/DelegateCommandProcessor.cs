using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Takenet.Text.Processors
{
    /// <summary>
    /// Processes commands using delegates.
    /// </summary>
    public class DelegateCommandProcessor : ICommandProcessor
    {
        private readonly Delegate _delegate;
        private readonly ParameterInfo[] _actionParameters;
        private readonly bool _allowNullOnNullableParameters;

        private DelegateCommandProcessor(Delegate @delegate, bool allowNullOnNullableParameters,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            if (syntaxes == null ||
                !syntaxes.Any())
            {
                throw new ArgumentNullException(nameof(syntaxes));
            }
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            if (!typeof(Task).IsAssignableFrom(@delegate.Method.ReturnType))
            {
                throw new ArgumentException("The delegate must return a Task");
            }

            if (outputProcessor != null &&
                !@delegate.Method.ReturnType.IsGenericType)
            {
                throw new ArgumentException("A delegate with return value is required to use with an output processor");
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

        public Task ProcessAsync(Expression expression)
        {
            var parameterArray = TypeUtil.GetParametersFromExpression(
                expression,
                _actionParameters,
                _allowNullOnNullableParameters);

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

        public static DelegateCommandProcessor Create<TResult>(Func<TResult> func, IOutputProcessor outputProcessor,
            params Syntax[] syntaxes)
        {
            return Create<TResult>(() => Task.FromResult(func()), outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, TResult>(Func<T1, TResult> func, IOutputProcessor outputProcessor,
            params Syntax[] syntaxes)
        {
            return Create<T1, TResult>((a) => Task.FromResult(func(a)), outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, TResult>(Func<T1, T2, TResult> func, IOutputProcessor outputProcessor,
           params Syntax[] syntaxes)
        {
            return Create<T1, T2, TResult>((a, b) => Task.FromResult(func(a, b)), outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2, T3, TResult>((a, b, c) => Task.FromResult(func(a, b, c)), outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2, T3, T4, TResult>((a, b, c, d) => Task.FromResult(func(a, b, c, d)), outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2, T3, T4, T5, TResult>((a, b, c, d, e) => Task.FromResult(func(a, b, c, d, e)), outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func,
            IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2, T3, T4, T5, T6, TResult>((a, b, c, d, e, f) => Task.FromResult(func(a, b, c, d, e, f)), outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create(Action action, IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create(() =>
            {
                action();
                return Task.FromResult(0);
            }, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1>(Action<T1> action, IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1>((a) =>
            {
                action(a);
                return Task.FromResult(0);
            }, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2>(Action<T1, T2> action, IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2>((a, b) =>
            {
                action(a, b);
                return Task.FromResult(0);
            }, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3>(Action<T1, T2, T3> action, IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2, T3>((a, b, c) =>
            {
                action(a, b, c);
                return Task.FromResult(0);
            }, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2, T3, T4>((a, b, c, d) =>
            {
                action(a, b, c, d);
                return Task.FromResult(0);
            }, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2, T3, T4, T5>((a, b, c, d, e) =>
            {
                action(a, b, c, d, e);
                return Task.FromResult(0);
            }, outputProcessor, syntaxes);
        }

        public static DelegateCommandProcessor Create<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, IOutputProcessor outputProcessor, params Syntax[] syntaxes)
        {
            return Create<T1, T2, T3, T4, T5, T6>((a, b, c, d, e, f) =>
            {
                action(a, b, c, d, e, f);
                return Task.FromResult(0);
            }, outputProcessor, syntaxes);
        }
    }
}