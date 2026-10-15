namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A second group joined to a combat restriction: "and/or black creatures" in "can't be blocked except by artifact creatures and/or black creatures".</summary>
/// <exampledoc>Arcbound Fiend</exampledoc>
/// <examplecapture>and/or black creatures</examplecapture>
[Dependent]
public class AndOrKind : Glyph
{
    public override Nib[] Nibs => ["and/or", Prop(Qualifiers), Plural(Prop(Kind))];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public PermanentKind Kind { get; set; }
}
