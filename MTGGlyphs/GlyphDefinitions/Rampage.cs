namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The rampage keyword with its bonus: "rampage 2".</summary>
/// <exampledoc>Chromium</exampledoc>
/// <examplecapture>rampage 2</examplecapture>
[Dependent]
public class Rampage : Glyph
{
    public override Nib[] Nibs => ["rampage", Prop(Bonus)];

    public Quantity Bonus { get; set; }
}
