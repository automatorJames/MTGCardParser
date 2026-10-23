namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of God-Eternal Kefnet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>God-Eternal Kefnet</exampledoc>
[TokenizationOrder(-1)]
public class GodEternalKefnetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that copy costs {2} less to cast"];
}
