using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Translator.Core.Domain;
using Translator.Core.Utility;

namespace Translator.Core.Translate
{
    // Read/write list item's data in several threads is safe.
    // Unsafe change list items count from several threads (Add / Remove etc.)
    public static class MultithreadMapTextTranslator
    {
        private const int MapObjectsPerWorker = 50;

        public static MapText Translate(MapText sourceText, ITranslateProccessor translator)
        {
            var translatedText = sourceText.CreateCopy();
            var tasks = new List<Task>();

            var t = new Task(() =>
            {
                // translatedText.Name.Data = translator.Translate(sourceText.Name.Data);

                translatedText.Description.Data = translator.Translate(sourceText.Description.Data);

                for (int i = 0; i < sourceText.RumorsCollection.Count; ++i)
                {
                    translatedText.RumorsCollection[i].Name.Data = translator.Translate(sourceText.RumorsCollection[i].Name.Data);
                    translatedText.RumorsCollection[i].Data.Data = translator.Translate(sourceText.RumorsCollection[i].Data.Data);
                }

                for (int i = 0; i < sourceText.EventsCollection.Count; ++i)
                {
                    translatedText.EventsCollection[i].Name.Data = translator.Translate(sourceText.EventsCollection[i].Name.Data);
                    translatedText.EventsCollection[i].Message.Data = translator.Translate(sourceText.EventsCollection[i].Message.Data);
                }

                for (int i = 0; i < sourceText.HeroesCollection.Count; ++i)
                {
                    if (sourceText.HeroesCollection[i].Name != null)
                    {
                        translatedText.HeroesCollection[i].Name.Data = translator.Translate(sourceText.HeroesCollection[i].Name.Data);
                    }

                    if (sourceText.HeroesCollection[i].Biography != null)
                    {
                        translatedText.HeroesCollection[i].Biography.Data = translator.Translate(sourceText.HeroesCollection[i].Biography.Data);
                    }
                }

            });

            tasks.Add(t);

            var workersCount = Math.Min(sourceText.ObjectsCollection.Count / MapObjectsPerWorker, 10);

            for (int i = 0; i < workersCount; ++i)
            {
                var j = i;
                var ot = new Task(() =>
                {
                    var k = j * MapObjectsPerWorker;
                    TranslateMapObjects(translatedText.ObjectsCollection, translator, sourceText.ObjectsCollection,
                        k, k + MapObjectsPerWorker);                    
                });

                tasks.Add(ot);
            }

            t = new Task(() =>
            {
                TranslateMapObjects(translatedText.ObjectsCollection, translator, sourceText.ObjectsCollection,
                    workersCount * MapObjectsPerWorker, sourceText.ObjectsCollection.Count);
            });

            tasks.Add(t);

            var whenAll = Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                task.Start();
            }

            whenAll.Wait();

            return translatedText;
        }

        private static void TranslateMapObjects(List<ObjectNodeBase> translatedMapObjectCollection,
            ITranslateProccessor translator, List<ObjectNodeBase> mapObjectCollection, int fromIndex, int toIndex)
        {
            for (int i = fromIndex; i < toIndex; ++i)
            {
                switch (mapObjectCollection[i])
                {
                    case TownNode town:
                        var translatedTown = (TownNode)translatedMapObjectCollection[i];

                        if (town.Name != null)
                        {
                            translatedTown.Name.Data = translator.Translate(town.Name.Data);
                        }

                        if (town.VisitedHero != null)
                        {
                            if (town.VisitedHero.Name != null)
                            {
                                translatedTown.VisitedHero.Name.Data = translator.Translate(town.VisitedHero.Name.Data);
                            }

                            if (town.VisitedHero.Biography != null)
                            {
                                translatedTown.VisitedHero.Biography.Data =
                                    translator.Translate(town.VisitedHero.Biography.Data);
                            }
                        }

                        if (town.TownEvents.Count > 0)
                        {
                            for (int j = 0; j < town.TownEvents.Count; ++j)
                            {
                                translatedTown.TownEvents[j].Name.Data = translator.Translate(town.TownEvents[j].Name.Data);

                                translatedTown.TownEvents[j].Message.Data =
                                    translator.Translate(town.TownEvents[j].Message.Data);
                            }
                        }

                        break;

                    case HeroesInPrisonNode hero:
                        var translatedHero = (HeroesInPrisonNode)translatedMapObjectCollection[i];

                        if (hero.Name != null)
                        {
                            translatedHero.Name.Data = translator.Translate(hero.Name.Data);
                        }

                        if (hero.Biography != null)
                        {
                            translatedHero.Biography.Data = translator.Translate(hero.Biography.Data);
                        }

                        break;

                    case MapObjectMessageNode mapObject:
                        var translatedMapObject = (MapObjectMessageNode)translatedMapObjectCollection[i];

                        for (int k = 0; k < mapObject.Messages.Count; ++k)
                        {
                            translatedMapObject.Messages[k] = translator.Translate(mapObject.Messages[k]);
                        }

                        break;
                }
            }
        }
    }
}
