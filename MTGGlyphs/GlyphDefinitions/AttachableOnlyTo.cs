namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A limit on what an equipment or aura can be attached to: "can be attached only to a creature with toughness 4 or greater", "can be attached only to a legendary creature".</summary>
/// <exampledoc>Gate Smasher</exampledoc>
/// <examplecapture>can be attached only to a creature with toughness 4 or greater</examplecapture>
[Dependent]
public class AttachableOnlyTo : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can be attached only to", Prop(Host)];

    public IndefinitePermanent Host { get; set; }
}
