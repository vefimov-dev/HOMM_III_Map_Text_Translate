using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Translator.Core.Translate
{
    public class CheckMapTranslateProccessor : TranslateProccessorBase
    {
        public ConcurrentBag<string> OversizedStrings { get; } = new ConcurrentBag<string>();

        public int MaxLengthForStringWithoutWarning { get; set; }

        public int TranslatedSymbolCount { get; set; }

        public int TranslationRequestsCount { get; protected set; }

        public override async Task<string> Translate(string data)
        {
            if (data.Length > this.MaxLengthForStringWithoutWarning)
            {
                var ws = data.Substring(0, Math.Min(30, data.Length));
                this.OversizedStrings.Add(ws);
            }

            ++this.TranslationRequestsCount;
            this.TranslatedSymbolCount += data.Length;

            return data;
        }        
    }
}