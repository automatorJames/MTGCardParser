namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Three or more targets of different kinds acted on together: "up to one target artifact, up to one target creature, up to one target enchantment, and up to one target planeswalker", or joined by "and/or".</summary>
/// <exampledoc>Urgent Necropsy</exampledoc>
/// <examplecapture>up to one target artifact, up to one target creature, up to one target enchantment, and up to one target planeswalker</examplecapture>
[Dependent]
public class TargetList : GlyphOneOf
{
    public ManyOf<SpecificTarget> List { get; set; }
    public TargetsAndOr AndOr { get; set; }
}
