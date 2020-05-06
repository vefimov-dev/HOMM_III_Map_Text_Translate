using System.Text;

namespace Translator.Core.Translate
{
    public class CustomSymbolsRemoveTranslateProccessor : ITranslateProccessor
    {
        private string[] symbolsToRemove;
        private ITranslateProccessor nexTranslateProccessor;

        public CustomSymbolsRemoveTranslateProccessor(string[] symbolsToRemove, ITranslateProccessor nexTranslateProccessor)
        {
            this.symbolsToRemove = symbolsToRemove;
            this.nexTranslateProccessor = nexTranslateProccessor;
        }

        public string Translate(string data)
        {
            var sb = new StringBuilder(data);

            foreach (var s in this.symbolsToRemove)
            {
                sb.Replace(s, "");
            }

            data = sb.ToString();
            return this.nexTranslateProccessor.Translate(data);
        }
    }
}
