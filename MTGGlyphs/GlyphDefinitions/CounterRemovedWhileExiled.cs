namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A suspend-style trigger condition on the card in exile: "a time counter is removed from this card while it's exiled". Resolved inside TriggeredAbility's condition.</summary>
/// <exampledoc>Aeon Chronicler</exampledoc>
/// <examplecapture>a time counter is removed from this card while it's exiled</examplecapture>
[Dependent]
public class CounterRemovedWhileExiled : Glyph
{
    public override Nib[] Nibs => ["a", Prop(Kind), "counter is removed from this card while it's exiled"];

    public CounterType Kind { get; set; }
}
