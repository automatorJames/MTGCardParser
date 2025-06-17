namespace MTGCardParser;

public class Card
{
    public int CardId { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public string ManaCost { get; set; }
    public string Types { get; set; }
    public string Supertypes { get; set; }
    public string Subtypes { get; set; }
    public string Keywords { get; set; }
    public string Power { get; set; }
    public string Toughness { get; set; }
    public string Loyalty { get; set; }
    public string SetCode { get; set; }
    public int SetSequence { get; set; }
}

