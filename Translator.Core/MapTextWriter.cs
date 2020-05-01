using System;
using System.Collections.Generic;
using Translator.Core.Domain;

namespace Translator.Core
{
    public static class MapTextWriter
    {
        public static string[] WriteMapText(MapText mapText, string[] lines)
        {
            lines[1] = mapText.Name.Data;
            lines[4] = mapText.Description.Data;

            int i = 8;

            foreach (var rumor in mapText.RumorsCollection)
            {
                ++i; // name
                lines[i++] = rumor.Name.Data;
                ++i; //title;
                lines[i++] = rumor.Data.Data;

                ++i; // empty line
            }

            i += 2; // event section title

            foreach (var eventNode in mapText.EventsCollection)
            {
                ++i; // name
                lines[i++] = eventNode.Name.Data;
                ++i; // message
                lines[i++] = eventNode.Message.Data;

                ++i; // empty line
            }

            i += 2; // heroes section title

            foreach (var hero in mapText.HeroesCollection)
            {
                if (hero.Name != null)
                {
                    ++i; // name
                    lines[i++] = hero.Name.Data;                    
                }

                if (hero.Biography != null)
                {
                    ++i; // bio
                    lines[i++] = hero.Biography.Data;
                }

                ++i; // empty line
            }

            i += 2; // objects section title                       

            foreach (var item in mapText.ObjectsCollection)
            {
                switch (item)
                {
                    case TownNode town:
                        ++i; // title

                        if (town.Name != null)
                        {
                            ++i; // name
                            lines[i++] = town.Name.Data;

                            ++i; // empty line
                        }

                        if (town.VisitedHero != null)
                        {
                            ++i; // visited hero title

                            if (town.VisitedHero.Name != null)
                            {
                                ++i; // name
                                lines[i++] = town.VisitedHero.Name.Data;                                
                            }

                            if (town.VisitedHero.Biography != null)
                            {
                                ++i; // bio
                                lines[i++] = town.VisitedHero.Biography.Data;
                            }

                            ++i; // empty line
                        }

                        if (town.TownEvents.Count > 0)
                        {
                            i += 2; // town events

                            foreach (var townEvent in town.TownEvents)
                            {
                                ++i; // name
                                lines[i++] = townEvent.Name.Data;

                                ++i; // message
                                lines[i++] = townEvent.Message.Data;

                                ++i; // empty line
                            }
                        }

                        break;

                    case HeroesInPrisonNode hero:
                        ++i; // title

                        if (hero.Name != null)
                        {
                            ++i; // name
                            lines[i++] = hero.Name.Data;                            
                        }

                        if (hero.Biography != null)
                        {
                            ++i; // bio
                            lines[i++] = hero.Biography.Data;                            
                        }

                        ++i; // empty line

                        break;

                    case MapObjectMessageNode mapObject:
                        ++i; // title
                        ++i; // message

                        foreach (var msg in mapObject.Messages)
                        {
                            lines[i++] = msg;
                        }

                        ++i; // empty line
                        break;

                    default:
                        throw new ArgumentException($"Bad type: {item.GetType()}");
                }
            }
            
            return lines;
        }
    }
}
