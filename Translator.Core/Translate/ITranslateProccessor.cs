using System.Collections.Generic;
using System.Threading.Tasks;

namespace Translator.Core.Translate
{
    public abstract class TranslateProccessorBase
    {
        // map name to code => "English" to "en";
        protected readonly Dictionary<string, string> languageMap = new Dictionary<string, string>();

        public virtual string SourceLangugageCode { get; set; }
        public virtual string TargetLangugageCode { get; set; }

        public List<TranslationErrorInfo> TranslationErrors { get; } = new List<TranslationErrorInfo>();

        public abstract Task<string> Translate(string data);

        public virtual IEnumerable<string> GetSupportedLanguages() => this.languageMap.Keys;

        public virtual bool SetTargetLanguage(string name)
        {
            if (this.languageMap.ContainsKey(name))
            {
                this.TargetLangugageCode = this.languageMap[name];
                return true;
            }

            return false;
        }

        public virtual bool SetSourceLangugage(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                this.SourceLangugageCode = null;
                return true;
            }

            if (this.languageMap.ContainsKey(name))
            {
                this.SourceLangugageCode = this.languageMap[name];
                return true;
            }

            return false;
        }
    }

    public class TranslationErrorInfo
    {
        public string Message { get; set; }

        public string TranslationData { get; set; }
    }
}
