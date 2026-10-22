namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many times something happened to an object, as an amount: "the number of times this creature was kicked", "the number of times it was kicked", "the number of times this creature has mutated", "the number of times that spell was kicked". Multikicker and mutate count these.</summary>
/// <exampledoc>Voyager Drake</exampledoc>
/// <examplecapture>the number of times this creature was kicked</examplecapture>
[Dependent]
public class NumberOfTimes : Glyph
{
    public override Nib[] Nibs => ["the number of times", Prop(Object), Prop(Event)];

    public OneOf<ThisObject, It, ThatStackObject> Object { get; set; }
    public ObjectHistoryEvent Event { get; set; }
}
