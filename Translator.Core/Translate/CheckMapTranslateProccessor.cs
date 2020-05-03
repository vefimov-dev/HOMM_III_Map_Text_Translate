using System;
using System.Collections.Concurrent;

namespace Translator.Core.Translate
{
    public class CheckMapTranslateProccessor : TranslateProccessor
    {
        public ConcurrentBag<string> OversizedStrings { get; } = new ConcurrentBag<string>();

        public int MaxLengthForStringWithoutWarning { get; set; }

        public override string Translate(string data)
        {
            if (data.Length > this.MaxLengthForStringWithoutWarning)
            {
                var ws = data.Substring(0, Math.Min(30, data.Length));
                this.OversizedStrings.Add(ws);
            }

            return base.Translate(data);
        }

        protected override string MakeTranslation(string data)
        {
            this.TranslatedSymbolCount += data.Length;
            return data;
        }
    }
}