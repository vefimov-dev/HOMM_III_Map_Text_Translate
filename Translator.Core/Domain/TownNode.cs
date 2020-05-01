using System.Collections.Generic;
using System.Text;

namespace Translator.Core.Domain
{
    public class TownNode : ObjectNodeBase
    {
        public TextNode Name { get; set; }

        public TownStartHeroesNode VisitedHero { get; set; }

        public List<TownEventNode> TownEvents { get; private set; }

        public TownNode(string title, string nameData)
        {
            this.Title = title;

            if (!string.IsNullOrEmpty(nameData))
            {
                this.Name = new TextNode { Title = TitleNames.TownNameTitle, Data = nameData };
            }

            this.TownEvents = new List<TownEventNode>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Town: {this.Name?.Data}. {this.Title}");

            if (this.VisitedHero != null)
            {
                sb.AppendLine($"Visited Hero: {this.VisitedHero}");
            }

            if (this.TownEvents.Count > 0)
            {
                sb.AppendLine("Town events:");

                foreach (var item in this.TownEvents)
                {
                    sb.AppendLine(item.ToString());
                }
            }

            return sb.ToString();
        }
    }

    public class TownEventNode
    {
        public TextNode Name { get; set; }

        public TextNode Message { get; set; }

        public TownEventNode(string nameData, string messageData)
        {
            this.Name = new TextNode { Title = TitleNames.TownEventNameTitle, Data = nameData };
            this.Message = new TextNode { Title = TitleNames.TownEventMessageTitle, Data = messageData };
        }

        public override string ToString()
        {
            return $"Town Event: {this.Name.Data}, Message: {this.Message.Data}";
        }
    }
}
