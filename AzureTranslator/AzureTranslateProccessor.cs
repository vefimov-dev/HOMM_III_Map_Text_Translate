using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Translator.Core.Translate;

namespace AzureTranslator
{
    public class AzureTranslateProccessor : TranslateProccessorBase
    {
        private const int MaxStringLengthForTranslation = 4995;
        private const int AttemptsCount = 3;

        private readonly string endpoint;
        private readonly string key;
        private readonly string region;

        private readonly Uri getLanguagesUrl;
        private readonly string transleteUrlPattern;
      
        private Uri transleteUrl;
        private string sourceLangugage;
        private string targetLangugage;

        public AzureTranslateProccessor(string endpoint, string key, string region)
        {
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Bad endpoint or key.");
            }

            this.endpoint = endpoint;
            this.key = key;
            this.region = region;

            this.getLanguagesUrl = new Uri(endpoint + "/languages?api-version=3.0&scope=translation");
            this.transleteUrlPattern = endpoint + "/translate?api-version=3.0&to={0}";

            this.Initialize();
        }       

        public override string SourceLangugageCode
        {
            get => this.sourceLangugage;
            set
            {
                this.sourceLangugage = value;
                this.SetTransaltionUrl();
            }
        }

        public override string TargetLangugageCode
        {
            get => this.targetLangugage;

            set
            {
                this.targetLangugage = value;
                this.SetTransaltionUrl();
            }
        }

        public override async Task<string> Translate(string data)
        {
            if (data.Length > MaxStringLengthForTranslation)
            {
                this.TranslationErrors.Add(new TranslationErrorInfo { Message = "Oversized string", TranslationData = data });
                data = data.Substring(0, MaxStringLengthForTranslation);
            }

            var body = new[] { new { Text = data } };
            var requestBody = JsonConvert.SerializeObject(body);
            var attempt = 0;
            string translation = data;

            while (attempt < AttemptsCount)
            {
                try
                {
                    translation = await this.GetTranslation(requestBody);
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

        private void Initialize()
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = this.getLanguagesUrl;
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.key);
                request.Headers.Add("Accept-Language", "en");

                var response = client.SendAsync(request).Result;
                var jsonStream = response.Content.ReadAsStreamAsync().Result;

                using (var reader = new StreamReader(jsonStream, UnicodeEncoding.UTF8))
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(reader.ReadToEnd());
                    var languages = result["translation"];

                    foreach (var item in languages)
                    {
                        this.languageMap[item.Value["name"]] = item.Key;
                    }
                }
            }
        }

        private async Task<string> GetTranslation(string requestBody)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = this.transleteUrl;
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.key);
                if (!string.IsNullOrEmpty(this.region))
                {
                    request.Headers.Add("Ocp-Apim-Subscription-Region", this.region);
                }

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                JArray jaresult;
                jaresult = JArray.Parse(responseBody);

                var translation = (string)jaresult[0].SelectToken("translations[0].text");

                return translation;
            }
        }

        private void SetTransaltionUrl()
        {
            if (string.IsNullOrEmpty(this.SourceLangugageCode))
            {
                this.transleteUrl = new Uri(string.Format(this.transleteUrlPattern, this.TargetLangugageCode));
            }
            else
            {
                this.transleteUrl = new Uri(
                    $"{ string.Format(this.transleteUrlPattern, this.TargetLangugageCode) }&from={this.SourceLangugageCode}");
            }
        }       
    }    
}