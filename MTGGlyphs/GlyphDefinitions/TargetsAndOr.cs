namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Targets of different kinds joined by "and/or": "up to one target artifact, up to one target creature, …, and/or up to one target land".</summary>
/// <exampledoc>Chaotic Transformation</exampledoc>
/// <examplecapture>up to one target artifact, up to one target creature, up to one target enchantment, up to one target planeswalker, and/or up to one target land</examplecapture>
[Dependent]
public class TargetsAndOr : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(First), Pattern(",? and/or "), Prop(Last)];

    public CompoundOf<SpecificTarget> First { get; set; }
    public SpecificTarget Last { get; set; }
}
