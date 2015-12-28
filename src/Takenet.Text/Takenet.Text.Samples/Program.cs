using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.Text.Csdl;
using Takenet.Text.Processors;

namespace Takenet.Text.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var textProcessor = CreateCalculatorTextProcessor();

            // Creates an empty context
            var context = new RequestContext();

            string inputText;
            do
            {
                Console.Write("Input > ");
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
                Console.WriteLine("Elapsed: {0} ms ({1} ticks)", sw.ElapsedMilliseconds, sw.ElapsedTicks);

            } while (!string.IsNullOrWhiteSpace(inputText));

        }


        static ITextProcessor CreateCalculatorTextProcessor()
        {
            // Fist, define the calculator methods
            Func<int, int, int> sumFunc = (a, b) => a + b;
            Func<int, int, int> subtractFunc = (a, b) => a - b;
            Func<int, int, int> multiplyFunc = (a, b) => a * b;
            Func<int, int, int> divideFunc = (a, b) => a / b;

            // After that, the syntaxes for all operations, using the CSDL parser:
                        
            // 1. Sum:            
            // a) The default syntax, for inputs like 'sum 1 and 2' or 'sum 3 4'
            var sumSyntax = CsdlParser.Parse("operation+:Word(sum) a:Integer :Word?(and) b:Integer");
            // b) The alternative syntax, for inputs like '3 plus 4'
            var alternativeSumSyntax = CsdlParser.Parse("a:Integer :Word(plus,more) b:Integer");

            // 2. Subtract:            
            // a) The default syntax, for inputs like 'subtract 2 from 3'
            var subtractSyntax = CsdlParser.Parse("operation+:Word(subtract,sub) b:Integer :Word(from) a:Integer");
            // b) The alternative syntax, for inputs like '5 minus 3'
            var alternativeSubtractSyntax = CsdlParser.Parse("a:Integer :Word(minus) b:Integer");

            // 3. Multiply:            
            // a) The default syntax, for inputs like 'multiply 3 and 3' or 'multiply 5 2'
            var multiplySyntax = CsdlParser.Parse("operation+:Word(multiply,mul) a:Integer :Word?(and) b:Integer");
            // b) The alternative syntax, for inputs like '6 times 2'
            var alternativeMultiplySyntax = CsdlParser.Parse("a:Integer :Word(times) b:Integer");

            // 4. Divide:            
            // a) The default syntax, for inputs like 'divide 3 by 3' or 'divide 10 2'
            var divideSyntax = CsdlParser.Parse("operation+:Word(divide,div) a:Integer :Word?(by) b:Integer");
            // b) The alternative syntax, for inputs like '6 by 2'
            var alternativeDivideSyntax = CsdlParser.Parse("a:Integer :Word(by) b:Integer");

            // Define a output processor that prints the command results to the console
            var outputProcessor = new DelegateOutputProcessor<int>((o, context) => Console.WriteLine($"Result: {o}"));
            
            // Now create the command processors, to bind the methods to the syntaxes
            var sumCommandProcessor = DelegateCommandProcessor.Create(
                sumFunc,
                outputProcessor,
                sumSyntax,
                alternativeSumSyntax
                );
            var subtractCommandProcessor = DelegateCommandProcessor.Create(
                subtractFunc,
                outputProcessor,
                subtractSyntax,
                alternativeSubtractSyntax
                );
            var multiplyCommandProcessor = DelegateCommandProcessor.Create(
                multiplyFunc,
                outputProcessor,
                multiplySyntax,
                alternativeMultiplySyntax
                );
            var divideCommandProcessor = DelegateCommandProcessor.Create(
                divideFunc,
                outputProcessor,
                divideSyntax,
                alternativeDivideSyntax
                );

            // Finally, create the text processor and register all command processors
            var textProcessor = new TextProcessor();
            textProcessor.AddCommandProcessor(sumCommandProcessor);
            textProcessor.AddCommandProcessor(subtractCommandProcessor);
            textProcessor.AddCommandProcessor(multiplyCommandProcessor);
            textProcessor.AddCommandProcessor(divideCommandProcessor);

            return textProcessor;
        }

    }
}
