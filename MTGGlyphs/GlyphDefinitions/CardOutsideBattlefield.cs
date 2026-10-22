namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card (or cards) or spell in a zone other than the battlefield: "card from your graveyard", "cards in a graveyard", "cards from a single graveyard", "card with mana value 3 or less from your graveyard", "card with power 2 or less from your graveyard", "cards from target player's graveyard", "card at random from your graveyard", "card an opponent owns from exile" (exile has no owner word).</summary>
/// <exampledoc>Argivian Archaeologist</exampledoc>
/// <examplecapture>card from your graveyard</examplecapture>
[Dependent]
public class CardOutsideBattlefield : Glyph
{
    public override Nib[] Nibs => [Plural(Alt("card", "spell")), Prop(Bound), Prop(AtRandom), Prop(Owner), Opt(Alt("in", "from")), Prop(Zone)];

    [Optional]
    public OneOf<ManaValueLimit, WithQualifier> Bound { get; set; }
    [Optional]
    [RegexPattern("at random")]
    public bool? AtRandom { get; set; }
    [Optional]
    public ControlledBy Owner { get; set; }
    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
}

 