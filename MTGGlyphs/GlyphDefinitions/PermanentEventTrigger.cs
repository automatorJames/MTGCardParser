namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A trigger on something happening to a particular permanent ({this}, "this creature" in reminder text, the enchanted permanent, one named earlier), to any permanent of a kind, or to the card itself or another like it: "when {this} dies, …", "whenever enchanted land becomes tapped, …", "whenever a land
/// enters the battlefield, …", "whenever a creature enters the battlefield under your control, …", "whenever {this} blocks or becomes blocked by a non-wall creature, …", "when that creature leaves the battlefield this turn, …", "whenever this creature or another ally you control enters, …", "whenever a creature dealt damage by this creature this turn dies, …". A permanent of a kind is a referent, so the effect can say "that land's controller".
/// </summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>whenever a land enters the battlefield, {this} deals 2 damage to that land's controller</examplecapture>
public class PermanentEventTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Subject), Prop(Event), Prop(Under), Prop(Period), ",", Prop(Effect)];

    public OneOf<ThisOrAnother, PermanentPhrase, OneOf<DamagedPermanent, IndefinitePermanent>> Subject { get; set; }
    public OneOf<CombatEncounter, PermanentEvent?> Event { get; set; }
    [Optional]
    public UnderControlOf Under { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
    public DynamicGlyph Effect { get; set; }
}
