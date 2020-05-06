﻿using System.Collections.Generic;
using System.Text;
using Translator.Core.Utility;

namespace Translator.Core.Translate
{
    public class CombineLinesTranslateProcessor : ITranslateProccessor
    {
        private const string NewTextLine = "\t";
        private const char NewTextLineChar = '\t';

        private readonly ITranslateProccessor nextTranslateProcessor;

        public CombineLinesTranslateProcessor(ITranslateProccessor nextTranslateProcessor)
        {
            this.nextTranslateProcessor = nextTranslateProcessor;
        }

        public string Translate(string data)
        {
            if (data.ContainsOrdinalIgnoreCase(NewTextLine))
            {
                return this.CombineLines(data);
            }
            else
            {
                return this.nextTranslateProcessor.Translate(data);
            }
        }

        private string CombineLines(string data)
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

                while (index < data.Length && data[index] == NewTextLineChar)
                {
                    ++index;
                }

                lines.Add(data.Substring(start, index - start));
                start = index;
            }

            var sb = new StringBuilder();

            for (int i = 0; i < lines.Count; i += 2)
            {
                sb.Append(lines[i]);
                sb.Append(" ");
            }

            var newLine = sb.ToString().Trim();

            var translated = this.nextTranslateProcessor.Translate(newLine);

            var separatorsCount = ((lines.Count + 1) >> 1);

            if (separatorsCount < 1)
            {
                return translated;
            }

            var averageStringLength = translated.Length / separatorsCount;
            var halfAverageStringLength = averageStringLength >> 1;
            --separatorsCount;
            sb.Clear();
            start = 0;

            for (int i = 0; i < separatorsCount; ++i)
            {
                end = (i + 1) * averageStringLength;
                for (int k = 0; k < halfAverageStringLength; ++k)
                {
                    if (char.IsWhiteSpace(translated[end + k]))
                    {
                        end += k;
                        break;
                    }

                    if (char.IsWhiteSpace(translated[end - k]))
                    {
                        end -= k;
                        break;
                    }
                }

                sb.Append(translated.Substring(start, end - start));
                sb.Append(NewTextLine);
                start = end + 1;
            }

            sb.Append(translated.Substring(start, translated.Length - start));

            return sb.ToString();
        }
    }
}
