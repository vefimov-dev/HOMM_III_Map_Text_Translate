using System.Collections.Generic;

namespace Translator.Core.Domain
{
    public class TitleNames
    {
        public static string MapName { get; private set; }
        public static string MapDescription { get; private set; }

        public static string RumorsSectionTitle { get; private set; }
        public static string RumorsNameTitle { get; private set; }
        public static string RumorsDataTitle { get; private set; }

        public static string EventsSectionTitle { get; private set; }
        public static string EventNameTitle { get; private set; }
        public static string EventMessageTitle { get; private set; }

        public static string HeroesSectionTitle { get; private set; }
        public static string HeroesBiographyTitle { get; private set; }
        public static string HeroesNameTitle { get; private set; }

        public static string ObjectsSectionTitle { get; private set; }

        public static string MapObjectMessageTitle { get; private set; }
        
        public static string HeroesInPrisonNameTitle { get; private set; }
        public static string HeroesInPrisonBiographyTitle { get; private set; }

        public static string EndOfFileTitle { get; private set; }

        public static string TownNameTitle { get; private set; }
        public static string TownExistStartHero { get; private set; }
        public static string TownEventsTitle { get; private set; }
        public static string TownEventNameTitle { get; private set; }
        public static string TownEventMessageTitle { get; private set; }

        public static string TownNameCastle { get; private set; }
        public static string TownNameNecropolis { get; private set; }
        public static string TownNameRampart { get; private set; }
        public static string TownNameDungeon { get; private set; }
        public static string TownNameTower { get; private set; }
        public static string TownNameInferno { get; private set; }
        public static string TownNameFortress { get; private set; }
        public static string TownNameStronghold { get; private set; }
        public static string TownNameConflux { get; private set; }

        public static string MapObjectPrison { get; private set; }

        public static void Initialize(Dictionary<string, string> data)
        {
            MapName = data[nameof(MapName)];
            MapDescription = data[nameof(MapDescription)];
            RumorsSectionTitle = data[nameof(RumorsSectionTitle)];
            RumorsNameTitle = data[nameof(RumorsNameTitle)];
            RumorsDataTitle = data[nameof(RumorsDataTitle)];
            EventsSectionTitle = data[nameof(EventsSectionTitle)];
            EventNameTitle = data[nameof(EventNameTitle)];
            EventMessageTitle = data[nameof(EventMessageTitle)];
            HeroesSectionTitle = data[nameof(HeroesSectionTitle)];
            HeroesBiographyTitle = data[nameof(HeroesBiographyTitle)];
            HeroesNameTitle = data[nameof(HeroesNameTitle)];
            ObjectsSectionTitle = data[nameof(ObjectsSectionTitle)];

            TownNameTitle = data[nameof(TownNameTitle)];
            TownEventsTitle = data[nameof(TownEventsTitle)];
            TownEventNameTitle = data[nameof(TownEventNameTitle)];
            TownEventMessageTitle = data[nameof(TownEventMessageTitle)];
            TownExistStartHero = data[nameof(TownExistStartHero)];

            MapObjectMessageTitle = data[nameof(MapObjectMessageTitle)];            
            HeroesInPrisonNameTitle = data[nameof(HeroesInPrisonNameTitle)];
            HeroesInPrisonBiographyTitle = data[nameof(HeroesInPrisonBiographyTitle)];
            EndOfFileTitle = data[nameof(EndOfFileTitle)];

            TownNameCastle = data[nameof(TownNameCastle)];
            TownNameNecropolis = data[nameof(TownNameNecropolis)];
            TownNameRampart = data[nameof(TownNameRampart)];
            TownNameDungeon = data[nameof(TownNameDungeon)];
            TownNameTower = data[nameof(TownNameTower)];
            TownNameInferno = data[nameof(TownNameInferno)];
            TownNameFortress = data[nameof(TownNameFortress)];
            TownNameStronghold = data[nameof(TownNameStronghold)];
            TownNameConflux = data[nameof(TownNameConflux)];
            MapObjectPrison = data[nameof(MapObjectPrison)];            
        }
    }
}