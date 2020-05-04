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

            var pathToText = "Paragon.txt";
            var valueLinesEncoding = Encoding.GetEncoding("Windows-1251");
           //var valueLinesEncoding = Encoding.GetEncoding("GB2312");

            var lines = File.ReadAllLines(pathToText, Encoding.Default);
            var valueLines = File.ReadAllLines(pathToText, valueLinesEncoding);

            var k = 11;
            if (k == 1)
            {
                #region CheckMapText

                var cmt = MapTextParser.ParseMap(lines);

                var ct = new CheckMapTranslateProccessor { MaxLengthForStringWithoutWarning = 4000 };

                var tt = MultithreadMapTextTranslator.Translate(cmt, ct);

                var write = MapTextWriter.WriteMapText(tt, lines);

                File.WriteAllLines($"Checked_{pathToText}", write, Encoding.Default);

                var checkInfo = new List<string>();

                checkInfo.Add($"Translated symbols: {ct.TranslatedSymbolCount}, Translate requests count: {ct.TranslationRequestsCount}");

                foreach (var s in ct.OversizedStrings)
                {
                    checkInfo.Add(s);
                }

                File.WriteAllLines($"Check_Info_{pathToText}", checkInfo, Encoding.Default);

                #endregion
            }
            else
            {
                #region Translate

                var mt = MapTextParser.ParseMap(lines, valueLines);

                var pathToCred = @"D:\Work\HOMM_III_Map_Text_Translate\azure.cred";
                var cred = File.ReadAllLines(pathToCred);
                var at = new AzureTranslateProccessor(cred[0], cred[1], cred.Length > 2 ? cred[2] : null)
                { TargetLangugage = "ru", };

                var start = DateTime.Now;
                var tt = MultithreadMapTextTranslator.Translate(mt, at);

                var end = DateTime.Now;

                var write = MapTextWriter.WriteMapText(tt, lines);

                File.WriteAllLines($"RU_{pathToText}", write, Encoding.Default);

                var checkInfo = new List<string>();

                checkInfo.Add($"Translation time: {(end - start).TotalSeconds} seconds");

                foreach (var s in at.TranslationErrors)
                {
                    checkInfo.Add($"{s.Message} {s.TranslationData}");
                }

                File.WriteAllLines($"Info_{pathToText}", checkInfo, Encoding.Default);

                #endregion
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
