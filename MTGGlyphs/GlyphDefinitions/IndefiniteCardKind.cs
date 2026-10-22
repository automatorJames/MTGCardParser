namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One card kind in a list of alternatives picked from a group: "a mount creature card", "a plains card", "garruk planeswalker card", "a card with {x} in its mana cost".</summary>
/// <exampledoc>Frontier Seeker</exampledoc>
/// <examplecapture>a mount creature card</examplecapture>
[Dependent]
public class IndefiniteCardKind : Glyph
{
    public override Nib[] Nibs => [Opt(Pattern("an?")), Prop(Qualifiers), "card", Prop(With)];

    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
