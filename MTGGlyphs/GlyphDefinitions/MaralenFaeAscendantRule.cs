namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Maralen, Fae Ascendant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maralen, Fae Ascendant</exampledoc>
/// <examplecapture>once each turn, you may cast a spell with mana value less than or equal to the number of elves and faeries you control from among cards exiled with maralen this turn without paying its mana cost</examplecapture>
[TokenizationOrder(-1)]
public class MaralenFaeAscendantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once each turn, you may cast a spell with mana value less than or equal to the number of elves and faeries you control from among cards exiled with maralen this turn without paying its mana cost"];
}
