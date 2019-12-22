using System;
using System.Threading;
using Takenet.Textc;
using Takenet.Textc.Processors;
using Xunit;

namespace Axaprj.Textc.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var context = new RequestContext();
            // Define a output processor that prints the command results to the console
            var outputProcessor = new DelegateOutputProcessor<int>(
                (o, ctx) =>
                Console.WriteLine($"Result: {o}")
            );
            var textProcessor = Calculator.CreateTextProcessor(outputProcessor);
            string inputText = "sum 5 3";
            try
            {
                var task = textProcessor.ProcessAsync(inputText, context, CancellationToken.None);
                task.Wait();
            }
            catch (MatchNotFoundException)
            {
                throw new InvalidOperationException("There's no match for the specified input");
            }
        }
    }
}
