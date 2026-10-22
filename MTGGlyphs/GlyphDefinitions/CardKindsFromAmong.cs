namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A list of card kinds, one of each to be picked out of a group just revealed or looked at: "a mount creature card or a plains card from among them", "a creature card or garruk planeswalker card from among them".</summary>
/// <exampledoc>Frontier Seeker</exampledoc>
/// <examplecapture>a mount creature card or a plains card from among them</examplecapture>
[Dependent]
public class CardKindsFromAmong : Glyph
{
    public override Nib[] Nibs => [Prop(Kinds), "from", Prop(Among)];

    public ManyOf<IndefiniteCardKind> Kinds { get; set; }
    public AmongThem Among { get; set; }
}
