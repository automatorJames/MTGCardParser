namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card (or cards) or spell in a zone other than the battlefield: "card from your graveyard", "cards in a graveyard", "cards from a single graveyard", "card with mana value 3 or less from your graveyard", "card with power 2 or less from your graveyard".</summary>
/// <exampledoc>Argivian Archaeologist</exampledoc>
/// <examplecapture>card from your graveyard</examplecapture>
[Dependent]
public class CardOutsideBattlefield : Glyph
{
    public override Nib[] Nibs => [Plural(Alt("card", "spell")), Prop(Bound), Opt(Alt("in", "from")), Prop(Whose), Prop(Zone)];

    [Optional]
    public OneOf<ManaValueLimit, WithQualifier> Bound { get; set; }
    public Whose? Whose { get; set; }
    public NonBattlefieldZone Zone { get; set; }
}

 