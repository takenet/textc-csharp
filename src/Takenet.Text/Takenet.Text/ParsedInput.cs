using System;
using System.Linq;
using System.Threading.Tasks;
using Takenet.Text.Processors;

namespace Takenet.Text
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

        public async Task SubmitAsync()
        {
            if (Expression.Context != null)
            {
                // Apply the contextual tokens
                foreach (var token in Expression.Tokens.Where(t =>
                    t.Template.IsContextual &&
                    t.Source == TokenSource.Input &&
                    t.Value != null))
                {
                    Expression.Context.SetVariable(token.Template.Name, token.Value);
                }
            }

            var task = Processor.ProcessAsync(Expression);
            await task.ConfigureAwait(false);

            if (Processor.OutputProcessor != null &&
                task.GetType().IsGenericType)
            {
                dynamic dynamicTask = task;
                object commandOutput = dynamicTask.Result;

                await Processor.OutputProcessor.ProcessOutputAsync(
                    commandOutput,
                    Expression.Context)
                    .ConfigureAwait(false);                
            }
        }
    }
}