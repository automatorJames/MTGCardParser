namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player event, as one trigger event of a <see cref="CompoundTrigger"/>: "when you cycle this card", "whenever you gain life".</summary>
/// <exampledoc>Naya Sojourners</exampledoc>
/// <examplecapture>when you cycle this card</examplecapture>
[Dependent]
public class PlayerTriggerEvent : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Event)];

    public PlayerEvent Event { get; set; }
}
