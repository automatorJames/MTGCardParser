namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A trigger on something happening to a particular permanent ({this}, "this creature" in reminder text, the enchanted permanent, one named earlier, a legend's short name), to any permanent of a kind, to one or more of a kind at once, or to the card itself or another like it: "when {this} dies, …", "whenever enchanted land becomes tapped, …", "whenever a land
/// enters the battlefield, …", "whenever a creature enters the battlefield under your control, …", "whenever {this} blocks or becomes blocked by a non-wall creature, …", "when that creature leaves the battlefield this turn, …", "whenever this creature or another ally you control enters, …", "whenever this creature or equipped creature deals combat damage to a player, …", "whenever a creature dealt damage by this creature this turn dies, …", "whenever one or more creatures you control deal combat damage to a player, …", "whenever crosis deals combat damage to a player, …", "whenever a creature is put into your graveyard from the battlefield, …", or to either of two events: "when this creature enters or dies, …", "whenever this creature enters or transforms into huntmaster of the fells, …", or to the card with others (battalion): "whenever this creature and at least two other creatures attack, …"; possibly only the first time each turn ("whenever aurelia attacks for the first time each turn, …"), or only while a condition holds ("whenever this creature attacks while saddled, …"). A permanent of a kind is a referent, so the effect can say "that land's controller".
/// </summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>whenever a land enters the battlefield, {this} deals 2 damage to that land's controller</examplecapture>
public class PermanentEventTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Subject), Prop(Event), Prop(Under), Prop(Period), ",", Prop(Effect)];

    public OneOf<ThisOrAnother, ThisOrAttached, ThisAndOthers, PermanentPhrase, DamagedPermanent, IndefinitePermanent, OneOrMorePermanents, ShortName> Subject { get; set; }
    public OneOf<CombatEncounter, PermanentEvent?, PutIntoGraveyardEvent, EntersOrTransformsInto, ManyOf<PermanentEvent?>> Event { get; set; }
    [Optional]
    public UnderControlOf Under { get; set; }
    [Optional]
    public OneOf<ThisPeriod, EventOccurrence?, WhileCondition> Period { get; set; }
    public DynamicGlyph Effect { get; set; }
}
