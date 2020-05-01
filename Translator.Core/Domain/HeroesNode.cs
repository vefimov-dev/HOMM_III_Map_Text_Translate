namespace Translator.Core.Domain
{
    public class HeroesNode
    {
        public TextNode Name { get; set; }

        public TextNode Biography { get; set; }

        public HeroesNode(string nameData, string biographyData)
        {
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
            return $"Name: {this.Name?.Data}, Bio: {this.Biography?.Data}";
        }
    }

    public class TownStartHeroesNode : HeroesNode
    {
        public string Title { get; set; }

        public TownStartHeroesNode(string title, string nameData, string biographyData) : base(nameData, biographyData)
        {
            this.Title = title;
        }
    }
}
