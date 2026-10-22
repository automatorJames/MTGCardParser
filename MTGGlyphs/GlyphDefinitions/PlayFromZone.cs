namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A standing permission to play or cast cards from a zone other than the hand: "play lands from your graveyard", "cast creature spells from the top of your library", "play lands and cast spells from the top of your library", "cast artifact spells and colorless spells from …", "cast this card from exile", "cast that card from your graveyard", or from a set-aside group: "play lands from among cards exiled with this enchantment" - possibly free ("cast spells from your hand without paying their mana costs", rebound's "cast this card from exile without paying its mana cost"), by way of an alternative-cost keyword ("cast this card from your graveyard using its blitz ability") or an alternative mana cost ("cast this card from your graveyard by paying {2}{w} rather than paying its mana cost"), or for as long as a condition holds ("cast this card from your graveyard as long as you control a zombie"). Used after "you may". A single permission is tried before a list, so "cast instant and sorcery spells" stays one.</summary>
/// <exampledoc>Crucible of Worlds</exampledoc>
/// <examplecapture>play lands from your graveyard</examplecapture>
[Dependent]
public class PlayFromZone : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Plays), "from", Prop(Source), Prop(Free), Prop(Using), Prop(Duration)];

    public OneOf<PlayPermission, ManyOf<PlayPermission>> Plays { get; set; }
    public OneOf<TopOfLibrary, PlayersZone, OneOf<NonBattlefieldZone?, AmongThem>> Source { get; set; }
    [RegexPattern("without paying (its|their) mana costs?")]
    public bool Free { get; set; }
    [Optional]
    public OneOf<UsingKeywordAbility, ByPayingInstead> Using { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
