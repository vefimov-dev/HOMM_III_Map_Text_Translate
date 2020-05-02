using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AzureTranslator;
using Translator.Core.Domain;

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

            //var lines = File.ReadAllLines("Paragon.txt", Encoding.Default);

            //var mt = MapTextParser.ParseMap(lines);

            //var tp = new TestTranslateProccessor { SourceLangugage = "ru", TargetLangugage = "en" };

            //var tt = MapTextTranslator.Translate(mt, tp);

            //var write = MapTextWriter.WriteMapText(mt, lines);

            //File.WriteAllLines("Paragon_.txt", write, Encoding.Default);

            var en = "It is hard to meet a person with a monosyllabic surname.  Rumor says such people are from a place like heaven with gold everywhere, and they all have magical abilities.  Few people actually know why Lim became a vagabond, while rumor says he is looking for a lost artifact for a friend.  Though people hardly meet him, one lady claims to have witnessed a battle between him and one of the best wizards on this continent, when the dark night was lighted as daytime by the endless spell castings.";
            var de = "Der weise Rat hat beschlossen, dass es von Nцten ist, ein zufriedenes Volk unter sich zu haben. Dies geschieht am einfachsten und unblutigsten, wenn Ihr dem Volk all ihre Mьhsam erernteten Ressurcen jeden halben Mondlauf zurьckgebt. Selbstverstдndlich nur die, die Ihr nicht gebraucht habt. Nur so kann man das Volk ohne militдrische Autoritдt sicher unter sich halten. Und Ihr werdet Soldaten wo anders dringender benцtigen, da kцnnt Ihr sicher sein! Deshalb beugt ihr euch widerwillig der Entscheidung des Rates. Ob ihr wollt oder nicht steht nicht zur Depatte.";
            var ch = "克拉尼奥是龙族中难得一见的龙预言师，银龙一族中年轻的银龙王。为了帮助尼古拉斯报仇，违背了龙神的神谕，月光龙城不再受到龙神的眷顾。";

            var cred = File.ReadAllLines(pathToCred);

            var at = new AzureTranslateProccessor(cred[0], cred[1], cred.Length > 2 ? cred[2] : null) { TargetLangugage = "ru" };

            var en_ru = at.Translate(en);
            var de_ru = at.Translate(de);
            var ch_ru = at.Translate(ch);

             Console.ReadLine();
        }
    }
}
