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

            var jsonkeyFile = @"D:\Work\HOMM_III_Map_Text_Translate\!google\Translation-cac6d6b217ec.json";

            //var t = "Наконец пришел ответ от узурпатора. Могрейну стоило огромного труда унять дрожь в руках, когда он разворачивал свиток. Гонцу, доставившему его явно было известно о содержании послания и потому он мерзко ухмылялся. \"Могрейн, сколько я себя помню, ты не отличался терпением и благоразумием.Еще когда я учил тебя тактике и стратегии, нападение было для тебя предпочтительней защиты.Но сейчас тебе стоит одуматься, ибо трон Империи мой. С каждым днем под мои знамена становятся новые воины, отступников вроде твоего брата и наемника Хааса в расчет просто не стоит брать.Явись на собрание лордов, что будет через два месяца, присягни на верность, и я не только пощажу твою жизнь, но и оставлю правителем Стоунхелма. Если же ты не прекратишь бессмысленную борьбу, то на тебя обрушится мощь всех лордов Империи, что объединятся под моим началом!\" Могрейн скомкал послание и бросил в огонь. У него оставалось ДВА месяца на то, чтобы отбить Вирилл, столицу Империи у Фордрагона.";

            //var pathToCred1 = @"D:\Work\HOMM_III_Map_Text_Translate\azure.cred";
            //var cred1 = File.ReadAllLines(pathToCred1);
            //var at1 = new AzureTranslateProccessor(cred1[0], cred1[1], cred1.Length > 2 ? cred1[2] : null)
            //{ TargetLangugage = "en" };
            //var pre = new AzurePrepareTextTranslateProccessor(at1);
            //var hhhh = new CombineLinesTranslateProcessor(pre);            

            //var t2 = hhhh.Translate(t);

            //return;

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
                { TargetLangugageCode = "en", SourceLangugageCode = "ru"};

                var azpt = new AzurePrepareTextTranslateProccessor(at);

                var clt = new CombineLinesTranslateProcessor(azpt);
               

                var start = DateTime.Now;
                //var tt = MultithreadMapTextTranslator.Translate(mt, at);
                 var tt = SequentialMapTextTranslator.Translate(mt, clt);

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
