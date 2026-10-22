namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Plot's reminder text, second sentence - or, said of a card plotted by another effect, "you may cast it as a sorcery on a later turn without paying its mana cost".</summary>
/// <exampledoc>Outcaster Trailblazer</exampledoc>
/// <examplecapture>cast it as a sorcery on a later turn without paying its mana cost</examplecapture>
public class PlotCast : Glyph
{
    public override Nib[] Nibs => [Opt("you may"), "cast it as a sorcery on a later turn without paying its mana cost"];
}
