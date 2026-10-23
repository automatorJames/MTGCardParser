namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ronin, Shadow Stalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ronin, Shadow Stalker</exampledoc>
[Dependent]
public class RoninShadowStalkerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature gets -4/-4 until end of turn"];
}
