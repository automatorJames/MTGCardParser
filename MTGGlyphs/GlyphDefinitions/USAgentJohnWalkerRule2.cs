namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of U.S.Agent, John Walker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>U.S.Agent, John Walker</exampledoc>
[TokenizationOrder(-1)]
public class USAgentJohnWalkerRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["s"];
}
