namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent or spell becoming a color, all colors, a basic land type, a type or color chosen earlier, a designation ("becomes saddled until end of turn", "it becomes renowned"), or a characteristic of the controller's choice, possibly for a while: "becomes colorless until end of turn", "becomes all colors until end of turn", "becomes the creature type of your choice until end of turn", "becomes that type until end of turn", "becomes an island until end of turn", "becomes blue in addition to its other colors". Replaces the older BecomesColor, whose subjects (incl. "target spell or permanent") are now Subject alternatives.</summary>
/// <exampledoc>Mistform Dreamer</exampledoc>
/// <examplecapture>becomes the creature type of your choice until end of turn</examplecapture>
[Dependent]
public class BecomesCharacteristic : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Value), Prop(InAddition), Prop(Duration)];

    public BecomeVerb Verb { get; set; }
    public OneOf<ManaColor?, TypeDesignation?, AllColors?, CharacteristicOfChoice, BecomesLandType, PermanentDesignation?> Value { get; set; }
    [RegexPattern("in addition to (its|their) other (colors|types|creature types)")]
    public bool InAddition { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
