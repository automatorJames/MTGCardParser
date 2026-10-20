namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"enchanted creature's", "equipped creature's": the possessive of the permanent an aura or equipment is attached to.</summary>
/// <exampledoc>Stupefying Touch</exampledoc>
/// <examplecapture>enchanted creature's</examplecapture>
[Dependent]
public class EnchantedPossessive : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), "'s"];

    public EnchantedPermanent Permanent { get; set; }
}
