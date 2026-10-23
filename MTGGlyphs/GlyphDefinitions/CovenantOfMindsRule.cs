namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Covenant of Minds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Covenant of Minds</exampledoc>
[TokenizationOrder(-1)]
public class CovenantOfMindsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent may choose to put those cards into your hand"];
}
