namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Absolute Virtue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Absolute Virtue</exampledoc>
[TokenizationOrder(-1)]
public class AbsoluteVirtueCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you have protection from each of your opponents"];
}
