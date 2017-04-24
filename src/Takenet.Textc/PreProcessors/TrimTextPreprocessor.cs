using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.PreProcessors
{
    public class TrimTextPreprocessor : ITextPreprocessor
    {
        public int Priority => 0;

        public Task<string> ProcessTextAsync(string text, IRequestContext context, CancellationToken cancellationToken)
            => Task.FromResult(text.Trim());
    }
}
