using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Translator.Core;
using Translator.Core.Domain;
using Translator.Core.Translate;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var eliPath = Path.Combine(Environment.CurrentDirectory, "EditorLanguageInfo", "Russian.eli");
            var eliLines = File.ReadAllLines(eliPath, Encoding.Default);
            var eliData = new Dictionary<string, string>();
            for (int i = 1; i < eliLines.Length; ++i)
            {
                var kv = eliLines[i].Split('#');
                eliData[kv[0]] = kv[1];
            }

            TitleNames.Initialize(eliData);
            
            var lines = File.ReadAllLines("xiedu.txt", Encoding.Default);

            var mt = MapTextParser.ParseMap(lines);

            var tt = MapTextTranslator.Translate(mt, new TestTranslateProccessor());

            var write = MapTextWriter.WriteMapText(mt, lines);

            File.WriteAllLines("Paragon_.txt", write, Encoding.Default);

           // Console.ReadLine();
        }
    }
}
