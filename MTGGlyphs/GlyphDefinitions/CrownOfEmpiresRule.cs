namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Crown of Empires, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crown of Empires</exampledoc>
[TokenizationOrder(-1)]
public class CrownOfEmpiresRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of that creature instead if you control artifacts named scepter of empires and throne of empires"];
}
