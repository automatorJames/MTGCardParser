namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Amplifire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Amplifire</exampledoc>
[TokenizationOrder(-1)]
public class AmplifireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, this creature's base power becomes twice that card's power and its base toughness becomes twice that card's toughness"];
}
