using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.Processors
{
    public class HttpCommandProcessor : ICommandProcessor
    {
        private readonly string _uriTemplate;
        private readonly string _method;
        private readonly IDictionary<string, string> _headers;

        public HttpCommandProcessor(string uriTemplate, string method, string bodyTemplate = null,
            IDictionary<string, string> headers = null, IOutputProcessor outputProcessor = null, params Syntax[] syntaxes)
        {
            if (uriTemplate == null) throw new ArgumentNullException(nameof(uriTemplate));
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (syntaxes.Length == 0) throw new ArgumentException("Argument is empty collection", nameof(syntaxes));

            _uriTemplate = uriTemplate;
            _method = method;
            _headers = headers;            
            OutputProcessor = outputProcessor;
            Syntaxes = syntaxes;
        }

        public Syntax[] Syntaxes { get; }

        public Task ProcessAsync(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IOutputProcessor OutputProcessor { get; }
    }
}
