using System.Collections.Generic;

namespace Translator.Core.Domain
{
    public class MapText
    {
        public TextNode Name { get; set; }

        public TextNode Description { get; set; }

        public List<RumorsNode> RumorsCollection { get; set; }

        public List<EventNode> EventsCollection { get; set; }

        public List<HeroesNode> HeroesCollection { get; set; }

        public List<ObjectNodeBase> ObjectsCollection { get; set; }

        public MapText()
        {
            this.Name = new TextNode { Title = TitleNames.MapName };
            this.Description = new TextNode { Title = TitleNames.MapDescription };
            this.RumorsCollection = new List<RumorsNode>();
            this.EventsCollection = new List<EventNode>();
            this.HeroesCollection = new List<HeroesNode>();
            this.ObjectsCollection = new List<ObjectNodeBase>();
        }
    }
}
