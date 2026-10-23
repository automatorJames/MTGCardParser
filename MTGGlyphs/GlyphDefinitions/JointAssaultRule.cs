namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Joint Assault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Joint Assault</exampledoc>
[TokenizationOrder(-1)]
public class JointAssaultRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's paired with a creature, that creature also gets +2/+2 until end of turn"];
}
