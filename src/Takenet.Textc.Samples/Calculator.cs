using System;
using System.Threading.Tasks;
using Takenet.Textc.Csdl;
using Takenet.Textc.Processors;

namespace Takenet.Textc.Samples
{
    public class Calculator
    {
        public static ITextProcessor CreateTextProcessor()
        {
            // Fist, define the calculator methods
            Func<int, int, Task<int>> sumFunc = (a, b) => Task.FromResult(a + b);
            Func<int, int, Task<int>> subtractFunc = (a, b) => Task.FromResult(a - b);
            Func<int, int, Task<int>> multiplyFunc = (a, b) => Task.FromResult(a * b);
            Func<int, int, Task<int>> divideFunc = (a, b) => Task.FromResult(a / b);
            
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
            var multiplySyntax = CsdlParser.Parse("operation+:Word(multiply,mul) a:Integer :Word?(and,by) b:Integer");
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
            var sumCommandProcessor = new DelegateCommandProcessor(
                sumFunc,
                true,
                outputProcessor,                
                sumSyntax, 
                alternativeSumSyntax
                );
            var subtractCommandProcessor = new DelegateCommandProcessor(
                subtractFunc,
                true,
                outputProcessor,
                subtractSyntax,
                alternativeSubtractSyntax
                );
            var multiplyCommandProcessor = new DelegateCommandProcessor(
                multiplyFunc,
                true,
                outputProcessor,
                multiplySyntax,
                alternativeMultiplySyntax
                );
            var divideCommandProcessor = new DelegateCommandProcessor(
                divideFunc,
                true,
                outputProcessor,
                divideSyntax,
                alternativeDivideSyntax
                );

            // Finally, create the text processor and register all command processors
            var textProcessor = new TextProcessor();
            textProcessor.CommandProcessors.Add(sumCommandProcessor);
            textProcessor.CommandProcessors.Add(subtractCommandProcessor);
            textProcessor.CommandProcessors.Add(multiplyCommandProcessor);
            textProcessor.CommandProcessors.Add(divideCommandProcessor);

            return textProcessor;
        }
    }
}