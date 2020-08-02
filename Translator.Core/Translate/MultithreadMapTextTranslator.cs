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

        public static MapText Translate(MapText sourceText, TranslateProccessorBase translator)
        {
            var translatedText = sourceText.CreateCopy();
            var tasks = new List<Task>();

            var t = new Task(async () =>
            {
                // translatedText.Name.Data = translator.Translate(sourceText.Name.Data);

                translatedText.Description.Data = await translator.Translate(sourceText.Description.Data);

                for (int i = 0; i < sourceText.RumorsCollection.Count; ++i)
                {
                    translatedText.RumorsCollection[i].Name.Data =
                         await translator.Translate(sourceText.RumorsCollection[i].Name.Data);

                    translatedText.RumorsCollection[i].Data.Data = 
                         await translator.Translate(sourceText.RumorsCollection[i].Data.Data);
                }

                for (int i = 0; i < sourceText.EventsCollection.Count; ++i)
                {
                    translatedText.EventsCollection[i].Name.Data =
                         await translator.Translate(sourceText.EventsCollection[i].Name.Data);

                    translatedText.EventsCollection[i].Message.Data =
                         await translator.Translate(sourceText.EventsCollection[i].Message.Data);
                }

                for (int i = 0; i < sourceText.HeroesCollection.Count; ++i)
                {
                    if (sourceText.HeroesCollection[i].Name != null)
                    {
                        translatedText.HeroesCollection[i].Name.Data =
                            await translator.Translate(sourceText.HeroesCollection[i].Name.Data);
                    }

                    if (sourceText.HeroesCollection[i].Biography != null)
                    {
                        translatedText.HeroesCollection[i].Biography.Data =
                            await translator.Translate(sourceText.HeroesCollection[i].Biography.Data);
                    }
                }

            });

            tasks.Add(t);

            var workersCount = Math.Min(sourceText.ObjectsCollection.Count / MapObjectsPerWorker, 10);

            for (int i = 0; i < workersCount; ++i)
            {
                var j = i;
                var ot = new Task(async () =>
                {
                    var k = j * MapObjectsPerWorker;
                    await TranslateMapObjects(translatedText.ObjectsCollection, translator,
                                              sourceText.ObjectsCollection, k, k + MapObjectsPerWorker);                    
                });

                tasks.Add(ot);
            }

            t = new Task(async () =>
            {
                await TranslateMapObjects(translatedText.ObjectsCollection, translator,
                                          sourceText.ObjectsCollection, 
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

        private static async Task TranslateMapObjects(List<ObjectNodeBase> translatedMapObjectCollection,
            TranslateProccessorBase translator, List<ObjectNodeBase> mapObjectCollection, int fromIndex, int toIndex)
        {
            for (int i = fromIndex; i < toIndex; ++i)
            {
                switch (mapObjectCollection[i])
                {
                    case TownNode town:
                        var translatedTown = (TownNode)translatedMapObjectCollection[i];

                        if (town.Name != null)
                        {
                            translatedTown.Name.Data = await translator.Translate(town.Name.Data);
                        }

                        if (town.VisitedHero != null)
                        {
                            if (town.VisitedHero.Name != null)
                            {
                                translatedTown.VisitedHero.Name.Data =
                                    await translator.Translate(town.VisitedHero.Name.Data);
                            }

                            if (town.VisitedHero.Biography != null)
                            {
                                translatedTown.VisitedHero.Biography.Data =
                                    await translator.Translate(town.VisitedHero.Biography.Data);
                            }
                        }

                        if (town.TownEvents.Count > 0)
                        {
                            for (int j = 0; j < town.TownEvents.Count; ++j)
                            {
                                translatedTown.TownEvents[j].Name.Data =
                                    await translator.Translate(town.TownEvents[j].Name.Data);

                                translatedTown.TownEvents[j].Message.Data =
                                    await translator.Translate(town.TownEvents[j].Message.Data);
                            }
                        }

                        break;

                    case HeroesInPrisonNode hero:
                        var translatedHero = (HeroesInPrisonNode)translatedMapObjectCollection[i];

                        if (hero.Name != null)
                        {
                            translatedHero.Name.Data = await translator.Translate(hero.Name.Data);
                        }

                        if (hero.Biography != null)
                        {
                            translatedHero.Biography.Data = await translator.Translate(hero.Biography.Data);
                        }

                        break;

                    case MapObjectMessageNode mapObject:
                        var translatedMapObject = (MapObjectMessageNode)translatedMapObjectCollection[i];

                        for (int k = 0; k < mapObject.Messages.Count; ++k)
                        {
                            translatedMapObject.Messages[k] = await translator.Translate(mapObject.Messages[k]);
                        }

                        break;
                }
            }
        }
    }
}
