namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A die's size, "d20": the number of faces.</summary>
/// <exampledoc>Djinni Windseer</exampledoc>
/// <examplecapture>d20</examplecapture>
[Dependent]
public class DieSize : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["d", Prop(Faces)];

    public int Faces { get; set; }
}
