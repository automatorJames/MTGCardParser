namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rocco, Street Chef, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rocco, Street Chef</exampledoc>
[TokenizationOrder(-1)]
public class RoccoStreetChefRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a player plays a land from exile or casts a spell from exile, you put a +1/+1 counter on target creature and create a food token"];
}
