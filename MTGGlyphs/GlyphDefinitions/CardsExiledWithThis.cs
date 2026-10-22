namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards this permanent exiled earlier, as one object or a group: "a card exiled with this creature", "all cards exiled with this artifact", "each creature card exiled with this land", "all other cards you own exiled with this enchantment", "cards exiled with this creature", "cards exiled with valgavoth", "creature cards exiled with it".</summary>
/// <exampledoc>Cold Storage</exampledoc>
/// <examplecapture>each creature card exiled with this artifact</examplecapture>
[Dependent]
[Referent]
public class CardsExiledWithThis : Glyph
{
    public override Nib[] Nibs => [Prop(Quantifier), Prop(Cards), Prop(YouOwn), "exiled with", Prop(Source)];

    [Optional]
    public ExiledCardsQuantifier? Quantifier { get; set; }
    public OneOf<QualifiedCards, CardsNoun?> Cards { get; set; }
    [Optional]
    [RegexPattern("you own")]
    public bool? YouOwn { get; set; }
    public OneOf<ThisCreature, This, OneOf<ShortNameObject, It>> Source { get; set; }
}
