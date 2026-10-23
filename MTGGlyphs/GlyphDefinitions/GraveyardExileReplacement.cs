namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement exiling a spell or card, or putting it elsewhere, instead of putting it into a graveyard: "if that spell is countered this way, exile it instead of putting it into its owner's graveyard", "if that spell would be put into your graveyard, exile it instead", "if a spell cast this way would be put into a graveyard, exile it instead", "if that spell would be put into a graveyard, put it on the bottom of its owner's library instead", "if a card would be put into your graveyard from anywhere this turn, exile that card instead".</summary>
/// <exampledoc>Assert Authority</exampledoc>
/// <examplecapture>if that spell is countered this way, exile it instead of putting it into its owner's graveyard</examplecapture>
public class GraveyardExileReplacement : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Spell), Prop(Event), ",", Prop(Replacement), "instead", Prop(OfPutting)];

    public OneOf<SpellReference?, IndefiniteCard> Spell { get; set; }
    public OneOf<CounteredThisWay, WouldGoToGraveyard> Event { get; set; }
    public OneOf<DestroyTarget, PutIntoZone> Replacement { get; set; }
    [RegexPattern("of putting it into its owner's graveyard")]
    public bool OfPutting { get; set; }
}
