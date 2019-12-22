using System;
using System.Threading.Tasks;
using Takenet.Textc;
using Takenet.Textc.Csdl;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;
using Takenet.Textc.Splitters;

namespace Axaprj.Textc.Tests
{
    public class Calculator
    {
        public static ITextProcessor CreateTextProcessor(DelegateOutputProcessor<int> outputProcessor)
        {
            // Fist, define the calculator methods
            Func<int, int, Task<int>> sumFunc = (a, b) => Task.FromResult(a + b);
            
            // After that, the syntaxes for all operations, using the CSDL parser:
            // 1. Sum:            
            // a) The default syntax, for inputs like 'sum 1 and 2' or 'sum 3 4'
            var sumSyntax = CsdlParser.Parse("operation+:VWord(sum) a:Integer :Word?(and) b:Integer");
            // b) The alternative syntax, for inputs like '3 plus 4'
            var alternativeSumSyntax = CsdlParser.Parse("a:Integer :Word(plus,more) b:Integer");
            
            // Now create the command processors, to bind the methods to the syntaxes
            var sumCommandProcessor = new DelegateCommandProcessor(
                sumFunc,
                true,
                outputProcessor,                
                sumSyntax, 
                alternativeSumSyntax
                );

            // Finally, create the text processor and register all command processors
            var textProcessor = new TextProcessor(new PunctuationTextSplitter());
            textProcessor.CommandProcessors.Add(sumCommandProcessor);

            textProcessor.TextPreprocessors.Add(new TrimTextPreprocessor());

            return textProcessor;
        }
    }
}