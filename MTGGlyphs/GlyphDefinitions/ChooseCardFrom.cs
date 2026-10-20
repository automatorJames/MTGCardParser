namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Choosing a card from cards just revealed or looked at: "choose a nonland card from it" (a revealed hand), "chooses a creature card from among them", "choose an instant or sorcery card from it". A predicate, so "you choose …" and "an opponent chooses …" resolve.</summary>
/// <exampledoc>Distress</exampledoc>
/// <examplecapture>choose a nonland card from it</examplecapture>
[Dependent]
public class ChooseCardFrom : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("chooses?"), Pattern("an?"), Prop(Qualifiers), "card from", Prop(Source)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>> Qualifiers { get; set; }
    public OneOf<It, AmongThem> Source { get; set; }
}
