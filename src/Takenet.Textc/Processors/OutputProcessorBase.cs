using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.Processors
{
    public abstract class OutputProcessorBase<TOutput> : IOutputProcessor
    {
        public Task ProcessOutputAsync(object output, IRequestContext context, CancellationToken cancellationToken)
        {
            return ProcessOutputAsync((TOutput)output, context, cancellationToken);
        }

        public abstract Task ProcessOutputAsync(TOutput output, IRequestContext context, CancellationToken cancellationToken);
    }
}