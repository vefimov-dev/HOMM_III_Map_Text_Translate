using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using Translator.Core.Translate;

namespace GoogleTranslator
{
    public class GoogleTranslateProccessor : TranslateProccessorBase
    {
        #region Fields

        private const int MaxStringLengthForTranslation = 29999;
        private const int AttemptsCount = 3;

        public readonly string quotString = "&quot;";
        public readonly string singleQuotString = "&#39;";
        
        public readonly string credentialsPath;
        private readonly string projectId;

        #endregion

        public GoogleTranslateProccessor(string jsonKeyFilePath, string projectId)
        {
            if (string.IsNullOrEmpty(jsonKeyFilePath) || !File.Exists(jsonKeyFilePath)
                || string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentException("Bad jsonKeyFilePath or projectId");
            }

            this.credentialsPath = jsonKeyFilePath;
            this.projectId = projectId;

            this.Initialize();
        }

        public override async Task<string> Translate(string data)
        {
            if (data.Length > MaxStringLengthForTranslation)
            {
                this.TranslationErrors.Add(new TranslationErrorInfo { Message = "Oversized string", TranslationData = data });
                data = data.Substring(0, MaxStringLengthForTranslation);
            }

            var attempt = 0;
            string translation = data;

            while (attempt < AttemptsCount)
            {
                try
                {
                    translation = await this.GetTranslation(data);
                    break;
                }
                catch (Exception ex)
                {
                    this.TranslationErrors.Add(new TranslationErrorInfo { Message = $"{ex.Message}, attempt: {attempt}", TranslationData = data });
                }

                ++attempt;
            }

            return translation;
        }

        private TranslationServiceClient GetTranslationServiceClient()
        {
            return new TranslationServiceClientBuilder
            {
                CredentialsPath = credentialsPath
            }.Build();
        }

        private LocationName GetLocation() => new LocationName(this.projectId, "global");

        private async Task<string> GetTranslation(string data)
        {
            var client = this.GetTranslationServiceClient();

            var request = new TranslateTextRequest
            {
                Contents = { data },
                TargetLanguageCode = TargetLangugageCode,
                ParentAsLocationName = this.GetLocation(),
            };

            var response = await client.TranslateTextAsync(request);

            var text = response.Translations[0].TranslatedText;

            var sb = new StringBuilder(text);
            sb.Replace(quotString, "\"");
            sb.Replace(singleQuotString, "'");

            return sb.ToString();
        }

        private async void Initialize()
        {
            var client = this.GetTranslationServiceClient();
            var request = new GetSupportedLanguagesRequest
            {
                ParentAsLocationName = this.GetLocation()
            };

            var response = await client.GetSupportedLanguagesAsync(request);
            foreach (var language in response.Languages)
            {
                this.languageMap[language.DisplayName] = language.LanguageCode;
            }
        }
    }
}
