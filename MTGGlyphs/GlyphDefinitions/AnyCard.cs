namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a card", "a creature card", "another card", "a nonland card with mana value 3 or less": any one card of a kind, as what a trigger watches or a reveal loop stops at.</summary>
/// <exampledoc>Along the Crooked Way</exampledoc>
/// <examplecapture>a creature card</examplecapture>
[Dependent]
public class AnyCard : Glyph
{
    public override Nib[] Nibs => [Pattern("(an?|another)"), Prop(Qualifiers), "card", Prop(With)];

    [Optional]
    public OneOf<CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
