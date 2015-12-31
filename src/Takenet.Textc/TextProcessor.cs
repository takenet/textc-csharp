using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
        private readonly List<ICommandProcessor> _commandProcessors;
        private readonly List<ITextPreprocessor> _textPreprocessors;

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
            if (syntaxParser == null) throw new ArgumentNullException(nameof(syntaxParser));
            if (expressionScorer == null) throw new ArgumentNullException(nameof(expressionScorer));

            SyntaxParser = syntaxParser;                        
            ExpressionScorer = expressionScorer;

            _commandProcessors = new List<ICommandProcessor>();
            _textPreprocessors = new List<ITextPreprocessor>();
        }

        public ICollection<ICommandProcessor> CommandProcessors => _commandProcessors;

        public ICollection<ITextPreprocessor> TextPreprocessors => _textPreprocessors;

        protected ISyntaxParser SyntaxParser { get; }

        protected IExpressionScorer ExpressionScorer { get; }

        public async Task ProcessAsync(string inputText, IRequestContext context, CancellationToken cancellationToken)
        {
            if (inputText == null) throw new ArgumentNullException(nameof(inputText));            
            if (string.IsNullOrWhiteSpace(inputText))
            {
                throw new ArgumentException("The input string must have a value", nameof(inputText));
            }
            if (context == null) throw new ArgumentNullException(nameof(context));

            var processedInputText = inputText;

            var textPreprocessors = new List<ITextPreprocessor>(TextPreprocessors).OrderBy(p => p.Priority);
            foreach (var preprocessor in textPreprocessors)
            {
                cancellationToken.ThrowIfCancellationRequested();

                processedInputText =
                    await preprocessor.ProcessTextAsync(processedInputText, context, cancellationToken).ConfigureAwait(false);
            }

            var parsedInputs = new List<ParsedInput>();
            var textCursor = new TextCursor(processedInputText, context);

            // Makes a copy of the list
            var commandProcessors = new List<ICommandProcessor>(_commandProcessors);
            foreach (var commandProcessor in commandProcessors)
            {
                // Gets all the syntaxes that are of the same culture of the context or are culture invariant
                var syntaxes =
                    commandProcessor.Syntaxes.Where(
                        s => s.Culture.Equals(context.Culture) || s.Culture.Equals(CultureInfo.InvariantCulture));

                foreach (var syntax in syntaxes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    textCursor.RightToLeftParsing = syntax.RightToLeftParsing;
                    textCursor.Reset();

                    Expression expression;                    
                    if (SyntaxParser.TryParse(textCursor, syntax, context, out expression))
                    {
                        var commandParsedQuery = new ParsedInput(expression, commandProcessor);
                        parsedInputs.Add(commandParsedQuery);
                        break;
                    }
                }
            }

            // Gets the more relevant expression accordingly to the expression scorer
            var parsedInput = parsedInputs
                .OrderByDescending(e => ExpressionScorer.GetScore(e.Expression))
                .ThenByDescending(e => e.Expression.Tokens.Count(t => t != null))
                .FirstOrDefault();

            if (parsedInput != null)
            {
                await parsedInput.SubmitAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new MatchNotFoundException(inputText);
            }
        }
    }
}