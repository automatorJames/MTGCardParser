namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"enchanted creature" / "equipped creature" / "fortified land": the permanent an aura, equipment or fortification is attached to. Its kind is a referent, for a later "that creature".</summary>
/// <exampledoc>Flight</exampledoc>
/// <examplecapture>enchanted creature</examplecapture>
[Dependent]
public class EnchantedPermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Attachment), Prop(Kind)];

    public AttachedBy Attachment { get; set; }
    [Referent]
    [Singular]
    public PermanentKind Kind { get; set; }
}
