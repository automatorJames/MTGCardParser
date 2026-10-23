namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of U.S.Agent, John Walker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>U.S.Agent, John Walker</exampledoc>
[TokenizationOrder(-1)]
public class USAgentJohnWalkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["agent enters, create a colorless equipment artifact token named sturdy shield with \"equipped creature gets +1/+2\" and equip {2}"];
}
