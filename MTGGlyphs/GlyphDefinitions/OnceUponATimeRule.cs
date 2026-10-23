namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Once Upon a Time, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Once Upon a Time</exampledoc>
[TokenizationOrder(-1)]
public class OnceUponATimeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell is the first spell you've cast this game, you may cast it without paying its mana cost"];
}
