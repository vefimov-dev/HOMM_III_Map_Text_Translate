using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GoogleTranslator;
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

            var pathToText = "ch_xiedu.txt";
            //var valueLinesEncoding = Encoding.GetEncoding("Windows-1251");
            var valueLinesEncoding = Encoding.GetEncoding("GB2312");

            var lines = File.ReadAllLines(pathToText, Encoding.Default);
            var valueLines = File.ReadAllLines(pathToText, valueLinesEncoding);


            //var t = "Наконец пришел ответ от узурпатора. Могрейну стоило огромного труда унять дрожь в руках, когда он разворачивал свиток. Гонцу, доставившему его явно было известно о содержании послания и потому он мерзко ухмылялся. \"Могрейн, сколько я себя помню, ты не отличался терпением и благоразумием.Еще когда я учил тебя тактике и стратегии, нападение было для тебя предпочтительней защиты.Но сейчас тебе стоит одуматься, ибо трон Империи мой. С каждым днем под мои знамена становятся новые воины, отступников вроде твоего брата и наемника Хааса в расчет просто не стоит брать.Явись на собрание лордов, что будет через два месяца, присягни на верность, и я не только пощажу твою жизнь, но и оставлю правителем Стоунхелма. Если же ты не прекратишь бессмысленную борьбу, то на тебя обрушится мощь всех лордов Империи, что объединятся под моим началом!\" Могрейн скомкал послание и бросил в огонь. У него оставалось ДВА месяца на то, чтобы отбить Вирилл, столицу Империи у Фордрагона.";
            //var t22 = "{近几百年来，随着神圣教会在大陆的不断发展，大陆各国逐渐接}	{受了格罗里亚作为大陆的名字，意为神的赞美诗。神圣历六百八}	{十二年，最强大的亡灵法师罗德里格斯带领着他的亡灵大军杀上}	{了俄狄神殿。是时，在俄狄神殿内，光明教会的圣女，光天使威}	{娜，刚刚从天界降临不久。由于身体上的弱点，威娜还无法与力}	{量已达到巅峰的亡灵法师相抗衡，以致于连灵魂也被罗德里格斯}	{所控制，被收入神器灵魂法珠内，无法返回天界。俄狄神殿被亡}	{灵亵渎，连长年笼罩神殿的圣光也消失了。罗德里格斯被愤怒的}	{天神降下审判之光，灰飞烟灭。号称光辉之神殿的俄狄神殿则被}	{净化之火燃为废墟。}";

            //var pathToCred1 = @"D:\Work\HOMM_III_Map_Text_Translate\!google\google.cred";
            //var cred1 = File.ReadAllLines(pathToCred1);
            //var t1 = new GoogleTranslateProccessor(cred1[0], cred1[1])
            //{ TargetLangugageCode = "en" };
            //var hhhh = new CombineLinesTranslateProccessor(t1);
            //var rrr = new RemoveNontranslatedSymbolsProccessor(hhhh);

            //var t2 = rrr.Translate(t22).Result;

            //return;

            var k = 12;
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

                var pathToCred = @"D:\Work\HOMM_III_Map_Text_Translate\!google\google.cred";
                var cred = File.ReadAllLines(pathToCred);
                var translate = new GoogleTranslateProccessor(cred[0], cred[1])
                { TargetLangugageCode = "ru" };
                //var translate = new CheckMapTranslateProccessor();

                var combine = new CombineLinesTranslateProccessor(translate);
                var removeNontranslated = new RemoveNontranslatedSymbolsProccessor(combine);

                var start = DateTime.Now;
                var tt = MultithreadMapTextTranslator.Translate(mt, removeNontranslated);
                //var tt = SequentialMapTextTranslator.Translate(mt, combine);

                var end = DateTime.Now;

                var write = MapTextWriter.WriteMapText(tt, lines);

                File.WriteAllLines($"RU_{pathToText}", write, Encoding.Default);

                var checkInfo = new List<string>();

                checkInfo.Add($"Translation time: {(end - start).TotalSeconds} seconds");

                foreach (var s in translate.TranslationErrors)
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
