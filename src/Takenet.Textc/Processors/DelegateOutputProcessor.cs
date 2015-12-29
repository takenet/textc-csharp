using System;
using System.Threading.Tasks;

namespace Takenet.Textc.Processors
{
    public sealed class DelegateOutputProcessor<T> : OutputProcessorBase<T>
    {
        private readonly Func<T, IRequestContext, Task> _func;

        public DelegateOutputProcessor(Action<T, IRequestContext> action)
            : this((o, c) => { action(o, c); return Task.FromResult(0); })
        {
            
        }

        public DelegateOutputProcessor(Func<T, IRequestContext, Task> func)
        {
            _func = func;
        }

        public override Task ProcessOutputAsync(T output, IRequestContext context)
        {
            return _func(output, context);
        }
    }
}
