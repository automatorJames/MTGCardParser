namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Runic Repetition, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runic Repetition</exampledoc>
[TokenizationOrder(-1)]
public class RunicRepetitionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target exiled card with flashback you own to your hand"];
}
