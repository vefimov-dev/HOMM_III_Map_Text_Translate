namespace Translator.Core.Translate
{
    public class TestTranslateProccessor : TranslateProccessor
    {
        protected override string MakeTranslation(string data)
        {
            this.TranslatedSymbolCount += data.Length;
            return data;
        }
    }
}