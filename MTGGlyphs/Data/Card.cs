namespace MTGGlyphs.Data;

public class Card : IDocument
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

    /// <summary>The card's colors as letters ("U,W"), where the data gives them; null where they follow from the mana cost.</summary>
    public string Colors { get; set; }

    /// <summary>A two-faced card's back face (a transforming card's night side, a split card's other half), with its own name, text and characteristics; null for a one-faced card.</summary>
    public Card BackFace { get; set; }

    /// <summary>A card's volume is its set, by set sequence.</summary>
    int? IDocument.Volume => SetSequence;
}

