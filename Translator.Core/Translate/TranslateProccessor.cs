using System;
using System.Collections.Generic;
using System.Text;
using Translator.Core.Utility;

namespace Translator.Core.Translate
{
    public abstract class TranslateProccessor
    {
        private const string NewTextLine = "\t";
        private const char NewTextLineChar = '\t';

        public string SourceLangugage { get; set; }
        public virtual string TargetLangugage { get; set; }

        public Dictionary<string, string> TranslationErrors { get; } = new Dictionary<string, string>();

        public string Translate(string data)
        {            
            if (data.ContainsOrdinalIgnoreCase(NewTextLine))
            {
                return this.TraslateByLine(data);
            }
            else
            {
                return this.MakeTranslation(data);
            }
        }

        protected abstract string MakeTranslation(string data);

        private string TraslateByLine(string data)
        {
            var lines = new List<string>();
            int index = 0, start = 0, end = 0;
            while (index < data.Length)
            {
                end = data.IndexOf(NewTextLine, start);
                if (end == -1)
                {
                    lines.Add(data.Substring(start, data.Length - start));
                    break;
                }

                lines.Add(data.Substring(start, end - start));
                index = start = end;

                while (data[index] == NewTextLineChar && index < data.Length)
                {
                    ++index;
                }

                lines.Add(data.Substring(start, index - start));
                start = index;
            }

            for (int i = 0; i < lines.Count; i += 2)
            {
                lines[i] = this.MakeTranslation(lines[i]);
            }

            var sb = new StringBuilder();
            for (int i = 0; i < lines.Count; ++i)
            {
                sb.Append(lines[i]);
            }

            return sb.ToString();
        }

    }
}
