namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Qualifiers joined by "and/or", two or a serial list: "instant and/or sorcery", "blue and/or black", "bird and/or soldier", "artifact, creature, and/or enchantment".</summary>
/// <exampledoc>Calculated Dismissal</exampledoc>
/// <examplecapture>instant and/or sorcery</examplecapture>
[Dependent]
public class AndOrQualifiers : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and/or", Prop(Second)];

    public OneOf<SerialQualifiers, TargetQualifier> First { get; set; }
    public TargetQualifier Second { get; set; }
}
