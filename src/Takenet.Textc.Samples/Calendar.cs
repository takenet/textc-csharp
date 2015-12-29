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
    public class Calendar
    {
        private readonly List<Reminder> _reminders;

        public Calendar()
        {
            _reminders = new List<Reminder>();
        }

        public Task<Reminder> AddReminderAsync(string message, string when)
        {            
            var reminder = new Reminder(message, when);
            _reminders.Add(reminder);            
            return Task.FromResult(reminder);            
        }

        public Task<IEnumerable<Reminder>> GetRemindersAsync(string when)
        {
            if (string.IsNullOrEmpty(when))
            {
                return Task.FromResult<IEnumerable<Reminder>>(_reminders);
            }

            return Task.FromResult(
                _reminders.Where(r => r.When.Equals(when, StringComparison.OrdinalIgnoreCase)));
        }

        public static ITextProcessor CreateTextProcessor()
        {
            // The calendar syntaxes, using some LDWords for input flexibility
            var addReminderSyntax = CsdlParser.Parse(
                "^[:Word?(hey,ok) :LDWord?(calendar,agenda) :LDWord(remind) :Word?(me) :Word~(to,of) message:Text when:LDWord?(today,tomorrow,someday)]");
            var getRemindersSyntax = CsdlParser.Parse(
                "[when:LDWord?(today,tomorrow,someday) :LDWord(reminders)]");

            // The output processors
            var addReminderOutputProcessor = new DelegateOutputProcessor<Reminder>((reminder, context) =>
            {
                Console.WriteLine($"Reminder '{reminder.Message}' added successfully for '{reminder.When}'");
            });
            var getRemindersOutputProcessor = new DelegateOutputProcessor<IEnumerable<Reminder>>((reminders, context) =>
            {
                var remindersDictionary = reminders
                    .GroupBy(r => r.When)
                    .ToDictionary(r => r.Key, r => r.Select(reminder => reminder.Message));

                foreach (var when in remindersDictionary.Keys)
                {
                    Console.WriteLine($"Reminders for {when}:");

                    foreach (var reminderMessage in remindersDictionary[when])
                    {
                        Console.WriteLine($"* {reminderMessage}");
                    }

                    Console.WriteLine();
                }
            });

            // Creating a instance to be shared by all processors
            var calendar = new Calendar();

            // The reflection processor
            var addRemiderCommandProcessor = new ReflectionCommandProcessor(
                calendar,
                nameof(AddReminderAsync),
                true,
                addReminderOutputProcessor,
                addReminderSyntax);

            var getRemidersCommandProcessor = new ReflectionCommandProcessor(
                calendar,
                nameof(GetRemindersAsync),
                true,
                getRemindersOutputProcessor,
                getRemindersSyntax);

            // Registering the processors
            var textProcessor = new TextProcessor();
            textProcessor.AddCommandProcessor(addRemiderCommandProcessor);
            textProcessor.AddCommandProcessor(getRemidersCommandProcessor);

            // Adding some pre-processors to normalize the input text
            textProcessor.AddTextPreProcessor(new TextNormalizerPreProcessor());
            textProcessor.AddTextPreProcessor(new ToLowerCasePreProcessor());

            return textProcessor;
        }

        public class Reminder
        {
            public Reminder(string message, string when)
            {
                Message = message;
                When = when ?? "someday";
            }

            public string Message { get; }

            public string When { get; }
        }
    }
}
