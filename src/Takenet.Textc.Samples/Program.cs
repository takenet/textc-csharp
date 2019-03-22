using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc.Csdl;
using Takenet.Textc.Processors;

namespace Takenet.Textc.Samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await MainAsync(args);
        }

        static async Task MainAsync(string[] args)
        {
            ITextProcessor textProcessor;

            Console.WriteLine("Select the sample: ");
            Console.WriteLine("1. Calculator");
            Console.WriteLine("2. Calendar");
            Console.WriteLine("3. Pizza");

            switch (Console.ReadLine()??"".Trim())
            {
                case "2":
                    Console.WriteLine("Starting the calendar...");
                    textProcessor = Calendar2.CreateTextProcessor();
                    break;

                case "3":
                    Console.WriteLine("Starting the pizza...");
                    textProcessor = Pizza.CreateTextProcessor();
                    break;

                default:
                    Console.WriteLine("Starting the calculator...");
                    textProcessor = Calculator.CreateTextProcessor();
                    break;
            }

            Console.Clear();

            // Creates an empty context
            var context = new RequestContext();

            string inputText;
            do
            {
                Console.WriteLine();
                Console.Write("> ");
                inputText = Console.ReadLine();

                var sw = Stopwatch.StartNew();

                try
                {
                    await textProcessor.ProcessAsync(inputText, context, CancellationToken.None);
                }
                catch (MatchNotFoundException)
                {
                    Console.WriteLine("There's no match for the specified input");
                }
                catch (ArgumentException)
                {
                    break;
                }

                sw.Stop();

#if DEBUG
                Console.WriteLine("Elapsed: {0} ms ({1} ticks)", sw.ElapsedMilliseconds, sw.ElapsedTicks);
#endif

            } while (!string.IsNullOrWhiteSpace(inputText));

        }
    }
}
