namespace Translator.Core.Domain
{
    [System.Serializable]
    public class TextNode
    {
        public string Title { get; set; }

        public string Data { get; set; }

        public override string ToString()
        {
            return $"Title: {this.Title}, Data: {this.Data}";
        }
    }
}
