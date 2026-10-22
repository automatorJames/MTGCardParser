namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of its colors", "of that spell's colors": each color an object has, as what a "for each" counts.</summary>
/// <exampledoc>Might of the Nephilim</exampledoc>
/// <examplecapture>of its colors</examplecapture>
[Dependent]
public class ColorsOfObject : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Object), "colors"];

    public OneOf<Its, ObjectPossessive> Object { get; set; }
}
