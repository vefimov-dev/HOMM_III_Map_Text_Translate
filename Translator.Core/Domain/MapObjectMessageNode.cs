using System.Collections.Generic;
using System.Text;

namespace Translator.Core.Domain
{
    public class MapObjectMessageNode : ObjectNodeBase
    {
        public List<string> Messages { get; set; }

        public MapObjectMessageNode(string title)
        {
            this.Title = title;
            this.Messages = new List<string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"MapObjectMessage: {this.Title}");

            foreach (var item in this.Messages)
            {
                sb.AppendLine(item);
            }

            return sb.ToString();
        }
    }
}
