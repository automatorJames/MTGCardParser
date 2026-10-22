namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of each creature you control": whose power and toughness DoublePowerToughness doubles, when it's written after them.</summary>
/// <exampledoc>Zopandrel, Hunger Dominus</exampledoc>
/// <examplecapture>of each creature you control</examplecapture>
[Dependent]
public class DoubledObjects : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Objects)];

    public PermanentPhrase Objects { get; set; }
}
