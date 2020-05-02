using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AzureTranslator;
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

            var pathToCred = @"D:\Work\HOMM_III_Map_Text_Translate\azure.cred";
            var pathToText = "xiedu.txt";
            //var encoding = Encoding.GetEncoding("Windows-1250");
            var encoding = Encoding.GetEncoding("GB2312");

            var lines = File.ReadAllLines(pathToText, Encoding.Default);
            var valueLines = File.ReadAllLines(pathToText, encoding);

            var mt = MapTextParser.ParseMap(lines, valueLines);

            var cred = File.ReadAllLines(pathToCred);
            var at = new AzureTranslateProccessor(cred[0], cred[1], cred.Length > 2 ? cred[2] : null) { TargetLangugage = "ru" };

            var tt = MapTextTranslator.Translate(mt, at);

            var write = MapTextWriter.WriteMapText(tt, lines);

            File.WriteAllLines($"RU_{pathToText}", write, Encoding.Default);

            // Console.ReadLine();
        }
    }
}
