using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Translator.Core.Translate;

namespace AzureTranslator
{
    public class AzurePrepareTextTranslateProccessor : TranslateProccessorBase
    {
        private readonly string[] symbolsToRemove = new[] { "{", "}", "\"", "*", "%" };
        private readonly string openQuotePattern = @"\W'";
        private readonly string closeQuotePattern = @"'\W";

        private readonly AzureTranslateProccessor nexTranslateProccessor;

        public AzurePrepareTextTranslateProccessor(AzureTranslateProccessor nexTranslateProccessor)
        {
            this.nexTranslateProccessor = nexTranslateProccessor;
        }
        
        public override async Task<string> Translate(string data)
        {
            data = Regex.Replace(data, openQuotePattern, " ");
            data = Regex.Replace(data, closeQuotePattern, " ");

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
