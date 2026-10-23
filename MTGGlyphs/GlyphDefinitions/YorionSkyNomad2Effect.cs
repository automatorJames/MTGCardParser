namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Yorion, Sky Nomad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yorion, Sky Nomad</exampledoc>
[Dependent]
public class YorionSkyNomad2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile any number of other nonland permanents you own and control"];
}
