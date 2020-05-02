using System.Collections.Generic;

namespace Translator.Core.Translate
{
    public class TestTranslateProccessor : TranslateProccessor
    {
        protected override string MakeTranslation(string data)
        {
            return data;
        }
    }
}
