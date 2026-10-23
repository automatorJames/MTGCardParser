namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spinneret and Spiderling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spinneret and Spiderling</exampledoc>
[TokenizationOrder(-1)]
public class SpinneretAndSpiderlingCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever", Nib.This, "deals 4 or more damage, exile the top card of your library"];
}
