namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Contested Game Ball, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Contested Game Ball</exampledoc>
[TokenizationOrder(-1)]
public class ContestedGameBallRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you're dealt combat damage, the attacking player gains control of this artifact and untaps it"];
}
