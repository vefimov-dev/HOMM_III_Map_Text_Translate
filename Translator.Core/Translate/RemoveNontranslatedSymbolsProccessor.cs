using System.Text;
using System.Threading.Tasks;

namespace Translator.Core.Translate
{
    /// <summary>
    /// Remove symbols like '{' or '}'
    /// These symbols are used to format text, but in translation they can break sentence
    /// </summary>
    public class RemoveNontranslatedSymbolsProccessor : TranslateProccessorBase
    {
        private readonly string[] symbolsToRemove = new[] { "{", "}" };

        private readonly TranslateProccessorBase nexTranslateProccessor;

        public RemoveNontranslatedSymbolsProccessor(TranslateProccessorBase nexTranslateProccessor)
        {
            this.nexTranslateProccessor = nexTranslateProccessor;
        }

        public override async Task<string> Translate(string data)
        {
            var sb = new StringBuilder(data);

            foreach (var s in this.symbolsToRemove)
            {
                sb.Replace(s, "");
            }

            data = sb.ToString();

            return await this.nexTranslateProccessor.Translate(data);
        }
    }
}
