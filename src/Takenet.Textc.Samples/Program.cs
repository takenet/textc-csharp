using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Takenet.Textc.Csdl;
using Takenet.Textc.Processors;

namespace Takenet.Textc.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var textProcessor = Calculator.CreateTextProcessor();

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
                    await textProcessor.ProcessAsync(inputText, context);
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
