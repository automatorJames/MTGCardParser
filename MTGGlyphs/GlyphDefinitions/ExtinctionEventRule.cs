namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Extinction Event, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Extinction Event</exampledoc>
[TokenizationOrder(-1)]
public class ExtinctionEventRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile each creature with mana value of the chosen quality"];
}
