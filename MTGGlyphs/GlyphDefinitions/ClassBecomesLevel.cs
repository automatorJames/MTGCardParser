namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A class reaching a level: "this class becomes level 2".</summary>
/// <exampledoc>Wizard Class</exampledoc>
/// <examplecapture>this class becomes level 2</examplecapture>
[Dependent]
public class ClassBecomesLevel : Glyph
{
    public override Nib[] Nibs => ["this class becomes level", Prop(Level)];

    public int Level { get; set; }
}
