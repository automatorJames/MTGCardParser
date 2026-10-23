namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rude Awakening, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rude Awakening</exampledoc>
[TokenizationOrder(-1)]
public class RudeAwakeningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• until end of turn, lands you control become 2/2 creatures that are still lands"];
}
