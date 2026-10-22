namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An extra amount added to a payment for each of something: "plus an additional {1} for each faerie you control".</summary>
/// <exampledoc>Spell Stutter</exampledoc>
/// <examplecapture>plus an additional {1} for each faerie you control</examplecapture>
[Dependent]
public class PlusAdditionalForEach : Glyph
{
    public override Nib[] Nibs => ["plus an additional", Prop(Cost), Prop(PerEach)];

    public ManaCost Cost { get; set; }
    public ForEach PerEach { get; set; }
}
