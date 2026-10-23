namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Suffocation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Suffocation</exampledoc>
[TokenizationOrder(-1)]
public class Suffocation2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cast this spell only if you were dealt damage this turn by a red instant or sorcery spell"];
}
