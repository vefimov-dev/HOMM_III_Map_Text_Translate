using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Translator.Core;
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
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            var lines = File.ReadAllLines("Way Home.txt", win1251);

            var mt = MapTextParser.ParseMap(lines);

            var write = MapTextWriter.WriteMapText(mt, lines);

            File.WriteAllLines("Paragon_.txt", write, win1251);

           // Console.ReadLine();
        }
    }
}
