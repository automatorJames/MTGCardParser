namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something happening to a permanent, as one trigger event of a <see cref="CompoundTrigger"/>: "when this creature dies", "when this artifact enters", "whenever another detective you control enters".</summary>
/// <exampledoc>Naya Sojourners</exampledoc>
/// <examplecapture>when this creature dies</examplecapture>
[Dependent]
public class PermanentTriggerEvent : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Subject), Prop(Event), Prop(Under)];

    public OneOf<ThisOrAnother, ThisAndOthers, PermanentPhrase, DamagedPermanent, IndefinitePermanent, OneOrMorePermanents, ShortName> Subject { get; set; }
    public OneOf<CombatEncounter, PermanentEvent?, PutIntoGraveyardEvent, ManyOf<PermanentEvent?>> Event { get; set; }
    [Optional]
    public UnderControlOf Under { get; set; }
}
