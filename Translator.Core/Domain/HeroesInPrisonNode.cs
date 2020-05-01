namespace Translator.Core.Domain
{
    [System.Serializable]
    public class HeroesInPrisonNode : ObjectNodeBase
    {
        public TextNode Name { get; set; }

        public TextNode Biography { get; set; }

        public HeroesInPrisonNode(string title, string nameData, string biographyData)
        {
            this.Title = title;

            if (!string.IsNullOrEmpty(nameData))
            {
                this.Name = new TextNode { Title = TitleNames.HeroesNameTitle, Data = nameData };
            }

            if (!string.IsNullOrEmpty(biographyData))
            {
                this.Biography = new TextNode { Title = TitleNames.HeroesBiographyTitle, Data = biographyData };
            }

        }

        public override string ToString()
        {
            return $"Name: {this.Name?.Data}, Bio: {this.Biography.Data}";
        }
    }
}
