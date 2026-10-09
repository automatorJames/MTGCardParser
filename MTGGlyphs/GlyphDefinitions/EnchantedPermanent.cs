namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"enchanted creature": the permanent an aura is attached to. Its kind is a referent, for a later "that creature".</summary>
/// <exampledoc>Flight</exampledoc>
/// <examplecapture>enchanted creature</examplecapture>
[Dependent]
public class EnchantedPermanent : Glyph
{
    public override Nib[] Nibs => ["enchanted", Prop(Kind)];

    [Referent]
    [Singular]
    public PermanentKind Kind { get; set; }
}
