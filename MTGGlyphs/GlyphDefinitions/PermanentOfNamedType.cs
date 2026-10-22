namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent of a type named earlier: "a land of that type", "a snow land of the chosen type", "a land of any of those types" - as chosen-landwalk reminders name what defending player controls.</summary>
/// <exampledoc>Giant Slug</exampledoc>
/// <examplecapture>a land of that type</examplecapture>
[Dependent]
public class PermanentOfNamedType : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), Prop(OfType)];

    public IndefinitePermanent Permanent { get; set; }
    public NamedTypeReference OfType { get; set; }
}
