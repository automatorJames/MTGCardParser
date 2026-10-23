namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ronin, Shadow Stalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ronin, Shadow Stalker</exampledoc>
[TokenizationOrder(-1)]
public class RoninShadowStalkerCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{t}, sacrifice an equipment attached to ronin: target creature gets -4/-4 until end of turn"];
}
