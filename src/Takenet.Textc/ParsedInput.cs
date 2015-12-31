using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc.Processors;

namespace Takenet.Textc
{
    internal sealed class ParsedInput
    {
        public ParsedInput(Expression expression, ICommandProcessor processor)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            Expression = expression;
            Processor = processor;
        }

        public Expression Expression { get; }

        public ICommandProcessor Processor { get; }

        public async Task SubmitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Expression.Context != null)
            {
                // Apply the contextual tokens
                foreach (var token in Expression.Tokens.Where(t =>
                    t != null &&
                    t.Type.IsContextual &&
                    t.Source == TokenSource.Input &&
                    t.Value != null))
                {
                    Expression.Context.SetVariable(token.Type.Name, token.Value);
                }
            }

            var task = Processor.ProcessAsync(Expression, cancellationToken);
            await task.ConfigureAwait(false);

            if (Processor.OutputProcessor != null &&
                task.GetType().IsGenericType)
            {
                dynamic dynamicTask = task;
                object commandOutput = dynamicTask.Result;

                await Processor.OutputProcessor.ProcessOutputAsync(
                    commandOutput,
                    Expression.Context,
                    cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}