namespace Translator.Core.Domain
{
    public class RumorsNode
    {
        public TextNode Name { get; set; }

        public TextNode Data { get; set; }

        public RumorsNode(string nameData, string dataData)
        {
            this.Name = new TextNode { Title = TitleNames.RumorsNameTitle, Data = nameData };
            this.Data = new TextNode { Title = TitleNames.RumorsDataTitle, Data = dataData };
        }

        public override string ToString()
        {
            return $"Name: {this.Name.Data}, Data: {this.Data.Data}";
        }
    }
}
