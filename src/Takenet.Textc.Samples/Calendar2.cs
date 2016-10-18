using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.Textc.Csdl;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;

namespace Takenet.Textc.Samples
{
    public class Calendar2
    {
        public Task<string> AddReminderAsync(string reminder)
            => AddReminderForDateAsync(reminder, "eventualmente");

        public Task<string> AddReminderForDateAsync(string reminder, string date)
            => AddReminderForDateAndTimeAsync(reminder, date, "manha");

        public async Task<string> AddReminderForDateAndTimeAsync(string reminder, string date, string time)
        {
            // TODO: Store the reminder for the specified date/time
            return $"O lembrete '{reminder}' foi adicionado para {date} no período da {time}";
        }
    
        public static ITextProcessor CreateTextProcessor()
        {
            // The parsed syntaxes
            var syntax1 = CsdlParser.Parse(
                ":Word(lembrar) :Word?(de) reminder:Text");
            var syntax2 = CsdlParser.Parse(
                ":Word(lembre) :Word?(me) date:Word?(hoje,amanha,eventualmente) :Word?(de) reminder:Text");
            var syntax3 = CsdlParser.Parse(
                ":Word?(me) :Word(lembre) :Word~(de) reminder:Text date:Word?(hoje,amanha,eventualmente) :Word?(a) time:Word?(manha,tarde,noite)");
            
            // The output processor handles the command method return value 
            var addReminderOutputProcessor = new DelegateOutputProcessor<string>(
                (text, context) => Console.WriteLine(text));

            var calendar = new Calendar2();
            var commandProcessor1 = new ReflectionCommandProcessor(
                calendar,
                nameof(AddReminderAsync),
                true,
                addReminderOutputProcessor,
                syntax1);
            var commandProcessor2 = new ReflectionCommandProcessor(
                calendar,
                nameof(AddReminderForDateAsync),
                true,
                addReminderOutputProcessor,
                syntax2);
            var commandProcessor3 = new ReflectionCommandProcessor(
                calendar,
                nameof(AddReminderForDateAndTimeAsync),
                true,
                addReminderOutputProcessor,
                syntax3);

            // Register the the processor
            var textProcessor = new TextProcessor();
            textProcessor.CommandProcessors.Add(commandProcessor1);
            textProcessor.CommandProcessors.Add(commandProcessor2);
            textProcessor.CommandProcessors.Add(commandProcessor3);

            // Add some preprocessors to normalize the input text
            textProcessor.TextPreprocessors.Add(new TextNormalizerPreprocessor());
            textProcessor.TextPreprocessors.Add(new ToLowerCasePreprocessor());

            return textProcessor;
        }
    }
}
