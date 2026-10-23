namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A second group joined to a combat restriction: "and/or black creatures" in "can't be blocked except by artifact creatures and/or black creatures", "and/or creatures that share a color with it" (intimidate), "and/or creatures with flying", "or walls" in "can't be blocked by knights or walls".</summary>
/// <exampledoc>Arcbound Fiend</exampledoc>
/// <examplecapture>and/or black creatures</examplecapture>
[Dependent]
public class AndOrKind : Glyph
{
    public override Nib[] Nibs => [Alt("and/or", "or"), Prop(Qualifiers), Plural(Prop(Kind)), Prop(Narrowed)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public PermanentKind Kind { get; set; }
    [Optional]
    public OneOf<WithQualifier, SharesAspectWith> Narrowed { get; set; }
}
