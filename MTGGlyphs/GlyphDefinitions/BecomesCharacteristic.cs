namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent or spell becoming a color, a basic land type, or a characteristic of the controller's choice, possibly for a while: "becomes colorless until end of turn", "becomes the creature type of your choice until end of turn", "becomes an island until end of turn", "becomes blue in addition to its other colors". Replaces the older BecomesColor, whose subjects (incl. "target spell or permanent") are now Subject alternatives.</summary>
/// <exampledoc>Mistform Dreamer</exampledoc>
/// <examplecapture>becomes the creature type of your choice until end of turn</examplecapture>
[Dependent]
public class BecomesCharacteristic : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Value), Prop(InAddition), Prop(Duration)];

    public BecomeVerb Verb { get; set; }
    public OneOf<ManaColor?, CharacteristicOfChoice, BecomesLandType> Value { get; set; }
    [RegexPattern("in addition to (its|their) other (colors|types|creature types)")]
    public bool InAddition { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
