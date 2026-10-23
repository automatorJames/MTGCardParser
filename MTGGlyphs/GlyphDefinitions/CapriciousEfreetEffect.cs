namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Capricious Efreet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Capricious Efreet</exampledoc>
[Dependent]
public class CapriciousEfreetEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target nonland permanent you control and up to two target nonland permanents you don't control"];
}
