namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A characteristic of one object, with the object after it: "the power of the exiled card", "the mana value of the discarded card", "the power of the creature tapped this way", "the mana value of that spell", "the power of target creature you control", "the power of the creature that died".</summary>
/// <exampledoc>Bishop of Binding</exampledoc>
/// <examplecapture>the power of the exiled card</examplecapture>
[Dependent]
public class CharacteristicOfObject : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Characteristic), "of", Prop(Object)];

    public Characteristic Characteristic { get; set; }
    public OneOf<ThatCardPhrase, HandledObject, ThatCard, TheAffectedThisWay, ThatStackObject, SpecificTarget, ObjectNamedByEvent> Object { get; set; }
}
