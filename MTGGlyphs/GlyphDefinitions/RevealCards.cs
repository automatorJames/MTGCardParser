namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Revealing a card picked out of a group: "reveal a creature card from among them" - usually followed by "and put it into your hand".</summary>
/// <exampledoc>Commune with Nature</exampledoc>
/// <examplecapture>reveal a creature card from among them</examplecapture>
[Dependent]
public class RevealCards : Glyph, IPredicate
{
    public override Nib[] Nibs => ["reveal", Prop(Cards)];

    public CardsFromAmong Cards { get; set; }
}
