using System.Threading.Tasks;

namespace Takenet.Text.Processors
{
    public abstract class OutputProcessorBase<TOutput> : IOutputProcessor
    {
        public Task ProcessOutputAsync(object output, IRequestContext context)
        {
            return ProcessOutputAsync((TOutput) output, context);
        }

        public abstract Task ProcessOutputAsync(TOutput output, IRequestContext context);
    }
}