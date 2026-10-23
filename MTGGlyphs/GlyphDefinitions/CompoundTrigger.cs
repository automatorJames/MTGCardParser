namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One effect on two triggers joined by "and": "when you cycle this card and when this creature dies, …", "when this creature enters and whenever you cast a spell with mana value 4 or greater, …", "whenever another detective you control enters and whenever a detective you control is turned face up, …", "at the beginning of your upkeep and whenever you cast a green spell, …", "when this artifact enters and at the beginning of your upkeep, …". Each half carries its own "when"/"whenever"/"at", so no half can stand in for a condition elsewhere.</summary>
/// <exampledoc>Naya Sojourners</exampledoc>
/// <examplecapture>when you cycle this card and when this creature dies, you may put a +1/+1 counter on target creature</examplecapture>
public class CompoundTrigger : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and", Prop(Second), ",", Prop(Effect)];

    public OneOf<PermanentTriggerEvent, PlayerTriggerEvent, OneOf<SpellTriggerEvent, AtOrUntilPlayerPhase>> First { get; set; }
    public OneOf<PermanentTriggerEvent, PlayerTriggerEvent, OneOf<SpellTriggerEvent, AtOrUntilPlayerPhase>> Second { get; set; }
    public DynamicGlyph Effect { get; set; }
}
