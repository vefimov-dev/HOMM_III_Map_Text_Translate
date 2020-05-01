namespace Translator.Core.Domain
{
    public class EventNode
    {
        public TextNode Name { get; set; }

        public TextNode Message { get; set; }

        public EventNode(string nameData, string messageData)
        {
            this.Name = new TextNode { Title = TitleNames.EventNameTitle, Data = nameData };
            this.Message = new TextNode { Title = TitleNames.EventMessageTitle, Data = messageData };
        }

        public override string ToString()
        {
            return $"Name: {this.Name.Data}, Data: {this.Message.Data}";
        }
    }
}
