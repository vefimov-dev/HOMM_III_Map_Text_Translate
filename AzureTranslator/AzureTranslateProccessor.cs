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
    public class AzureTranslateProccessor : TranslateProccessor
    {
        private readonly string endpoint;
        private readonly string key;
        private readonly string region;

        private readonly Uri getLanguagesUrl;
        private readonly string transleteUrlPattern;

        // map name to code => "English" to "en";
        private readonly Dictionary<string, string> languageMap = new Dictionary<string, string>();

        private Uri transleteUrl;

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

            Initialize();
        }

        public override string TargetLangugage
        {
            get => base.TargetLangugage;

            set
            {
                base.TargetLangugage = value;
                this.transleteUrl = new Uri(string.Format(this.transleteUrlPattern, value));
            }
        }
        
        public IEnumerable<string> GetLanguages() => this.languageMap.Keys;

        public bool SetTargetLanguage(string nativeLanguageName)
        {
            if (this.languageMap.ContainsKey(nativeLanguageName))
            {
                this.TargetLangugage = this.languageMap[nativeLanguageName];
                return true;
            }

            return false;
        }

        protected override string MakeTranslation(string data)
        {
            var body = new[] { new { Text = data } };
            var requestBody = JsonConvert.SerializeObject(body);

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

                var response = client.SendAsync(request).Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;

                JArray jaresult;
                jaresult = JArray.Parse(responseBody);

                var translation = (string)jaresult[0].SelectToken("translations[0].text");

                this.TranslatedSymbolCount += data.Length;
                return translation;
            }
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
                        this.languageMap[item.Value["nativeName"]] = item.Key;
                    }
                }
            }
        }
    }
}