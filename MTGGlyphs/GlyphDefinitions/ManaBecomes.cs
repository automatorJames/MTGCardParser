namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mana changing its color or type, as a replacement: "that mana becomes colorless", "that mana becomes black".</summary>
/// <exampledoc>Kruphix, God of Horizons</exampledoc>
/// <examplecapture>that mana becomes colorless</examplecapture>
public class ManaBecomes : Glyph
{
    public override Nib[] Nibs => ["that mana becomes", Prop(Color)];

    public ManaColor Color { get; set; }
}
