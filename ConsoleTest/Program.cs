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

            var pathToText = "Metataxer's Revenge.txt";
            var valueLinesEncoding = Encoding.GetEncoding("Windows-1250");

            var lines = File.ReadAllLines(pathToText, Encoding.Default);
            var valueLines = File.ReadAllLines(pathToText, valueLinesEncoding);

            #region CheckMapText

            File.WriteAllLines($"Original_{pathToText}", lines, Encoding.Default);

            var cmt = MapTextParser.ParseMap(lines);

            var ct = new CheckMapTranslateProccessor { MaxLengthForStringWithoutWarning = 4000 };

            var tt = MapTextTranslator.Translate(cmt, ct);

            var write = MapTextWriter.WriteMapText(tt, lines);

            File.WriteAllLines($"Checked_{pathToText}", write, Encoding.Default);

            var checkInfo = new List<string>();

            checkInfo.Add($"Translated symbols: {ct.TranslatedSymbolCount}");

            foreach (var s in ct.OversizedStrings)
            {
                checkInfo.Add(s);
            }

            File.WriteAllLines($"Check_Info_{pathToText}", checkInfo, Encoding.Default);

            #endregion

            #region Translate

            //var mt = MapTextParser.ParseMap(lines, valueLines);

            //var pathToCred = @"D:\Work\HOMM_III_Map_Text_Translate\azure.cred";
            //var cred = File.ReadAllLines(pathToCred);
            //var at = new AzureTranslateProccessor(cred[0], cred[1], cred.Length > 2 ? cred[2] : null) { TargetLangugage = "ru" };

            //var tt = MapTextTranslator.Translate(mt, at);

            //var write = MapTextWriter.WriteMapText(tt, lines);

            //File.WriteAllLines($"RU_{pathToText}", write, Encoding.Default);

            #endregion

            Console.WriteLine("Done");
        }
    }
}
