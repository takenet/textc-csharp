using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;
using Takenet.Textc.Scorers;

namespace Takenet.Textc
{
    /// <summary>
    /// The default <see cref="ITextProcessor" /> implementation.
    /// </summary>
    public class TextProcessor : ITextProcessor
    {
        protected ICollection<ICommandProcessor> CommandProcessors { get; }

        protected ICollection<ITextPreProcessor> TextPreProcessors { get; }

        protected ISyntaxParser SyntaxParser { get; }

        protected IExpressionScorer ExpressionScorer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessor" /> class.
        /// </summary>
        public TextProcessor()
            : this(new SyntaxParser(), new ExpressionScorer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessor" /> class.
        /// </summary>
        public TextProcessor(ISyntaxParser syntaxParser, IExpressionScorer expressionScorer)
        {
            if (syntaxParser == null)
            {
                throw new ArgumentNullException(nameof(syntaxParser));
            }
            SyntaxParser = syntaxParser;


            if (expressionScorer == null)
            {
                throw new ArgumentNullException(nameof(expressionScorer));
            }
            ExpressionScorer = expressionScorer;

            CommandProcessors = new List<ICommandProcessor>();
            TextPreProcessors = new List<ITextPreProcessor>();
        }

        public void AddTextPreProcessor(ITextPreProcessor textPreProcessor)
        {
            TextPreProcessors.Add(textPreProcessor);
        }

        public void AddCommandProcessor(ICommandProcessor commandProcessor)
        {
            CommandProcessors.Add(commandProcessor);
        }

        public async Task ProcessAsync(string inputText, IRequestContext context)
        {
            if (inputText == null) throw new ArgumentNullException(nameof(inputText));
            if (string.IsNullOrWhiteSpace(inputText))
            {
                throw new ArgumentException("The input string must have a value", nameof(inputText));
            }

            var processedInputText = inputText;

            foreach (var preprocessor in TextPreProcessors.OrderBy(p => p.Priority))
            {
                processedInputText =
                    await preprocessor.ProcessTextAsync(processedInputText, context).ConfigureAwait(false);
            }

            var parsedInputList = new List<ParsedInput>();
            var textCursor = new TextCursor(processedInputText, context);

            foreach (var commandProcessor in CommandProcessors)
            {
                foreach (var syntax in commandProcessor.Syntaxes.Where(s => s.Culture.Equals(context.Culture) || s.Culture.Equals(CultureInfo.InvariantCulture)))
                {
                    textCursor.RightToLeftParsing = syntax.RightToLeftParsing;
                    Expression expression;
                    textCursor.Reset();

                    if (SyntaxParser.TryParse(textCursor, syntax, context, out expression))
                    {
                        var commandParsedQuery = new ParsedInput(expression, commandProcessor);
                        parsedInputList.Add(commandParsedQuery);
                        break;
                    }
                }
            }

            // Gets the more relevant expression accordingly to the expression scorer
            var parsedInput = parsedInputList
                .OrderByDescending(e => ExpressionScorer.GetScore(e.Expression))
                .ThenByDescending(e => e.Expression.Tokens.Count(t => t != null))
                .FirstOrDefault();

            if (parsedInput != null)
            {
                await parsedInput.SubmitAsync().ConfigureAwait(false);
            }
            else
            {
                throw new MatchNotFoundException(inputText);
            }
        }
    }
}