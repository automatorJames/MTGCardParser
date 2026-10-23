namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spinneret and Spiderling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spinneret and Spiderling</exampledoc>
[Dependent]
public class SpinneretAndSpiderlingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the top card of your library"];
}
