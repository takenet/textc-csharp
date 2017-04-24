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
        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessor" /> class.
        /// </summary>
        public TextProcessor()
            : this(new SyntaxParser(), new RatioExpressionScorer())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessor" /> class.
        /// </summary>
        public TextProcessor(ITextSplitter textSplitter)
            : this(new SyntaxParser(), new RatioExpressionScorer(), textSplitter)
        {
            if (textSplitter == null)
                throw new ArgumentNullException(nameof(textSplitter));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessor" /> class.
        /// </summary>
        public TextProcessor(ISyntaxParser syntaxParser, IExpressionScorer expressionScorer, ITextSplitter textSplitter = null)
        {
            SyntaxParser = syntaxParser ?? throw new ArgumentNullException(nameof(syntaxParser));
            ExpressionScorer = expressionScorer ?? throw new ArgumentNullException(nameof(expressionScorer));
            TextSplitter = textSplitter;
            CommandProcessors = new List<ICommandProcessor>();
            TextPreprocessors = new List<ITextPreprocessor>();
        }

        public ICollection<ICommandProcessor> CommandProcessors { get; }

        public ICollection<ITextPreprocessor> TextPreprocessors { get; }

        protected ITextSplitter TextSplitter { get; }

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

            foreach (var splittedInputText in SplitInput(inputText))
            {
                var parsedInputs = await ParseInput(splittedInputText, context, cancellationToken);
                if (!parsedInputs.Any()) throw new MatchNotFoundException(splittedInputText);

                // Gets the more relevant expression accordingly to the expression scorer
                var parsedInput = parsedInputs
                    .OrderByDescending(e => ExpressionScorer.GetScore(e.Expression))
                    .ThenByDescending(e => e.Expression.Tokens.Count(t => t != null))
                    .First();

                await parsedInput.SubmitAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private IEnumerable<string> SplitInput(string inputText)
        {
            if (TextSplitter != null) return TextSplitter.Split(inputText);
            return new[] { inputText };
        }

        private async Task<List<ParsedInput>> ParseInput(string inputText, IRequestContext context, CancellationToken cancellationToken)
        {
            var parsedInputs = new List<ParsedInput>();
            var processedInputText = inputText;
            var textPreprocessors = TextPreprocessors.ToList().OrderBy(p => p.Priority);
            foreach (var preprocessor in textPreprocessors)
            {
                cancellationToken.ThrowIfCancellationRequested();
                processedInputText =
                    await
                        preprocessor.ProcessTextAsync(processedInputText, context, cancellationToken)
                            .ConfigureAwait(false);
            }

            var textCursor = new TextCursor(processedInputText, context);

            foreach (var commandProcessor in CommandProcessors.ToList())
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
            return parsedInputs;
        }
    }
}