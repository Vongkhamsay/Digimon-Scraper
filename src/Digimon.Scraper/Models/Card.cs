namespace Digimon.Scraper.Models
{
  public class Card
  {
    public string Name { get; set; }
    public string Number { get; set; }
    public string CardType { get; set; }
    public string Level { get; set; }
    public string ImageUrl { get; set; }
    public string Form { get; set; }
    public string Attribute { get; set; }
    public string Type { get; set; }
    public bool IsAlternate { get; set; } = false;
    public string Dp { get; set; }
    public string PlayCost { get; set; }
    public string Digivolve1Cost { get; set; }
    public string Digivolve2Cost { get; set; }
    public string Effect { get; set; }
    public string DigivolveEffect { get; set; }
    public string SecurityEffect { get; set; }
  }
}
