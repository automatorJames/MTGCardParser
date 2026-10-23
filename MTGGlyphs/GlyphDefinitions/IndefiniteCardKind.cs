namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One card kind in a list of alternatives picked from a group: "a mount creature card", "a plains card", "garruk planeswalker card", "a card with {x} in its mana cost".</summary>
/// <exampledoc>Frontier Seeker</exampledoc>
/// <examplecapture>a mount creature card</examplecapture>
[Dependent]
public class IndefiniteCardKind : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "card", Prop(With)];

    public OneOf<ArticledCardKind, CompoundOf<QualifierWords>> Kind { get; set; }
    [Optional]
    public OneOf<WithQualifier, SymbolInManaCost> With { get; set; }
}
