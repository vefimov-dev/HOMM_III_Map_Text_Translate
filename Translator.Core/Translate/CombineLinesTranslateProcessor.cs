using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Translator.Core.Utility;

namespace Translator.Core.Translate
{
    public class CombineLinesTranslateProcessor : TranslateProccessorBase
    {
        private const string NewTextLine = "\t";
        private const char NewTextLineChar = '\t';

        private readonly TranslateProccessorBase nextTranslateProcessor;

        public CombineLinesTranslateProcessor(TranslateProccessorBase nextTranslateProcessor)
        {
            this.nextTranslateProcessor = nextTranslateProcessor;
        }

        string t = "Его младший брат слишко легко попадал под влияние сильных фигур";

        public override async Task<string> Translate(string data)
        {
            if (data.ContainsOrdinalIgnoreCase(t))
            {
                var f = 5;
            }

            if (data.ContainsOrdinalIgnoreCase(NewTextLine))
            {
                return await this.CombineLines(data);
            }
            else
            {
                return await this.nextTranslateProcessor.Translate(data);
            }
        }

        private async Task<string> CombineLines(string data)
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

            var translated = await this.nextTranslateProcessor.Translate(newLine);

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
