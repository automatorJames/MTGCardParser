namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One card of a kind each, picked out of a group: "a creature card and/or a land card from among them" - up to one of each.</summary>
/// <exampledoc>In the Presence of Ages</exampledoc>
/// <examplecapture>a creature card and/or a land card from among them</examplecapture>
[Dependent]
public class CardOfEachKindFromAmong : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and/or", Prop(Second), "from", Prop(Among)];

    public SingleCardOfKind First { get; set; }
    public SingleCardOfKind Second { get; set; }
    public AmongThem Among { get; set; }
}
