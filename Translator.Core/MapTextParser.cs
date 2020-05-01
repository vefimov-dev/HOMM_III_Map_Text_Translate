using System;
using Translator.Core.Domain;
using Translator.Core.Utility;

namespace Translator.Core
{
    public static class MapTextParser
    {
        public static MapText ParseMap(string[] lines)
        {
            var mt = new MapText();

            mt.Name.Data = lines[1];            
            mt.Description.Data = lines[4];

            var currentLine = 8; // rummors start
            int eventStartLine, heroesStartline, objectsStartLine, endFile = 0;

            eventStartLine = FindIndex(lines, currentLine + 2, x => x.IsOrdinalEqualsIgnoreCase(TitleNames.EventsSectionTitle));
            heroesStartline = FindIndex(lines, eventStartLine + 2, x => x.IsOrdinalEqualsIgnoreCase(TitleNames.HeroesSectionTitle));
            objectsStartLine = FindIndex(lines, heroesStartline + 2, x => x.IsOrdinalEqualsIgnoreCase(TitleNames.ObjectsSectionTitle));

            for (int i = lines.Length-1; i> 0; --i)
            {
                if (lines[i].IsOrdinalEqualsIgnoreCase(TitleNames.EndOfFileTitle))
                {
                    endFile = i;
                    break;
                }
            }

            // parse rummors
            while (currentLine < eventStartLine)
            {
                ++currentLine; // to name data

                var name = lines[currentLine];

                currentLine += 2; // to data 

                var data = lines[currentLine];

                currentLine += 2; // next rummor

                var rummor = new RumorsNode(name, data);
                mt.RumorsCollection.Add(rummor);
            }

            // parse events
            currentLine += 2;
            while (currentLine < heroesStartline)
            {
                ++currentLine; // to name data

                var name = lines[currentLine];

                currentLine += 2; // to data 

                var message = lines[currentLine];

                currentLine += 2; // next rummor

                var eventNode = new EventNode(name, message);
                mt.EventsCollection.Add(eventNode);
            }

            // parse heroes
            currentLine += 2;
            while (currentLine < objectsStartLine)
            {
                var nb = GetHeroNameAndBio(lines, ref currentLine);
                var hero = new HeroesNode(nb.Item1, nb.Item2);
                mt.HeroesCollection.Add(hero);
            }

            // parse objects
            currentLine += 2;

            while (currentLine < endFile)
            {
                var title = lines[currentLine];
                Tuple<string, string> nb;

                if (currentLine > 811)
                {
                    var d = 44;
                }

                switch (GetObjectType(lines, currentLine))
                {
                    case ObjectType.Town:
                        string townName = null;
                        ++currentLine;
                        
                        if (lines[currentLine].IsOrdinalEqualsIgnoreCase(TitleNames.TownNameTitle))
                        {
                            ++currentLine;
                            townName = lines[currentLine];
                            currentLine += 2;
                        }
                        else
                        {
                            ++currentLine;
                        }

                        var townNode = new TownNode(title, townName);

                        if (lines[currentLine].ContainsOrdinalIgnoreCase(TitleNames.TownExistStartHero))
                        {
                            title = lines[currentLine];
                            ++currentLine;
                            nb = GetHeroNameAndBio(lines, ref currentLine);

                            townNode.VisitedHero = new TownStartHeroesNode(title, nb.Item1, nb.Item2);
                        }

                        if (lines[currentLine].IsOrdinalEqualsIgnoreCase(TitleNames.TownEventsTitle))
                        {
                            currentLine += 2;
                            string en = null, em = null;
                            while (true)
                            {
                                if (lines[currentLine].IsOrdinalEqualsIgnoreCase(TitleNames.TownEventNameTitle))
                                {
                                    ++currentLine;
                                    en = lines[currentLine];

                                    currentLine += 2;
                                    em = lines[currentLine];

                                    townNode.TownEvents.Add(new TownEventNode(en, em));
                                    currentLine += 2;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        mt.ObjectsCollection.Add(townNode);

                        break;

                    case ObjectType.Hero:
                        ++currentLine;
                        nb = GetHeroNameAndBio(lines, ref currentLine);

                        var prison = new HeroesInPrisonNode(title, nb.Item1, nb.Item2);
                        mt.ObjectsCollection.Add(prison);
                        break;

                    case ObjectType.Message:
                        currentLine += 2;

                        var endMessage = 0;
                        var mom = new MapObjectMessageNode(title);
                        for (int i = currentLine; i < lines.Length; ++i)
                        {
                            if ((lines[i].StartsWithOrdinalIgnoreCase("(") && lines[i].EndsWithOrdinalIgnoreCase("***"))
                                || lines[i].StartsWithOrdinalIgnoreCase("="))
                            {
                                endMessage = i - 1;
                                break;
                            }
                        }

                        for (int i = currentLine; i < endMessage; ++i)
                        {
                            mom.Messages.Add(lines[i]);
                            ++currentLine;
                        }

                        ++currentLine;
                        mt.ObjectsCollection.Add(mom);
                        break;

                    default:
                        throw new ArgumentException("Bad ObjectType");
                }
            }

            return mt;
        }

        private static int FindIndex(string[] lines, int startIndex, Predicate<string> predicate)
        {
            for (int i = startIndex; i < lines.Length; ++i)
            {
                if (predicate(lines[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private enum ObjectType
        {
            Town,
            Hero, // in prison or on map
            Message
        }

        private static ObjectType GetObjectType(string[] lines, int currentIndex)
        {
            var title = lines[currentIndex];

            if (title.ContainsOrdinalIgnoreCase(TitleNames.MapObjectPrison))
            {
                return ObjectType.Hero;
            }
            
            if (title.ContainsOrdinalIgnoreCase(TitleNames.TownNameCastle) ||
                title.ContainsOrdinalIgnoreCase(TitleNames.TownNameConflux) ||
                title.ContainsOrdinalIgnoreCase(TitleNames.TownNameDungeon) ||
                title.ContainsOrdinalIgnoreCase(TitleNames.TownNameFortress) ||
                title.ContainsOrdinalIgnoreCase(TitleNames.TownNameInferno) ||
                title.ContainsOrdinalIgnoreCase(TitleNames.TownNameNecropolis) ||
                title.ContainsOrdinalIgnoreCase(TitleNames.TownNameRampart) ||
                title.ContainsOrdinalIgnoreCase(TitleNames.TownNameStronghold) ||
                title.ContainsOrdinalIgnoreCase(TitleNames.TownNameTower))
            {
                return ObjectType.Town;
            }

            if (lines[currentIndex + 1].IsOrdinalEqualsIgnoreCase(TitleNames.HeroesNameTitle) ||
                lines[currentIndex + 1].IsOrdinalEqualsIgnoreCase(TitleNames.HeroesBiographyTitle))
            {
                return ObjectType.Hero;
            }

            return ObjectType.Message;
        }

        private static Tuple<string, string> GetHeroNameAndBio(string[] lines, ref int index)
        {
            string name = null, bio = null;

            if (TitleNames.HeroesNameTitle.IsOrdinalEqualsIgnoreCase(lines[index]))
            {
                // has name
                ++index;
                name = lines[index];

                ++index;
                // also has bio
                if (TitleNames.HeroesBiographyTitle.IsOrdinalEqualsIgnoreCase(lines[index]))
                {
                    ++index;

                    bio = lines[index];

                    index += 2;
                }
                else
                {
                    ++index;
                }

                return new Tuple<string, string>(name, bio);
            }

            if (TitleNames.HeroesBiographyTitle.IsOrdinalEqualsIgnoreCase(lines[index]))
            {
                // has bio
                ++index;

                bio = lines[index];

                index += 2;
            }

            return new Tuple<string, string>(name, bio);
        }
    }
}
