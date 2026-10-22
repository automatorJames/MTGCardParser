namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"there are four or more card types among cards in your graveyard" (delirium), "there are five basic land types among lands you control" (domain), "there are five colors among permanents you control", "there are five or more mana values among cards in your graveyard": how many distinct kinds a zone's cards or a group of permanents span.</summary>
/// <exampledoc>Gnarlwood Dryad</exampledoc>
/// <examplecapture>there are four or more card types among cards in your graveyard</examplecapture>
[Dependent]
public class CardTypesCountCondition : Glyph
{
    public override Nib[] Nibs => ["there are", Prop(Count), Prop(Kinds), "among", Prop(Among)];

    public OneOf<AtLeastCount, Quantity?> Count { get; set; }
    public CountedAspect Kinds { get; set; }
    public OneOf<CardsInPlayersZone, Permanents> Among { get; set; }
}
