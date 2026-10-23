namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"instead, rounded down": a replacement's closing word followed by how a halved number rounds. The comma is a literal here, since a nested glyph can't open with one.</summary>
/// <exampledoc>Vorinclex, Monstrous Raider</exampledoc>
/// <examplecapture>instead, rounded down</examplecapture>
[Dependent]
public class InsteadRounded : Glyph
{
    public override Nib[] Nibs => ["instead,", Prop(Rounded)];

    public Rounding Rounded { get; set; }
}
