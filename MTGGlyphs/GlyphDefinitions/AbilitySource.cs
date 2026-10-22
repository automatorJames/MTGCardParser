namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which permanent an ability belongs to: "of a creature", "of a sarkhan planeswalker".</summary>
/// <exampledoc>Sarkhan's Whelp</exampledoc>
/// <examplecapture>of a sarkhan planeswalker</examplecapture>
[Dependent]
public class AbilitySource : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Permanent)];

    public IndefinitePermanent Permanent { get; set; }
}
