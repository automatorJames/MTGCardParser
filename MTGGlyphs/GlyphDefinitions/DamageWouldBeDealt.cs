namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage about to be dealt, as a replacement condition: "damage would be dealt to this creature", "combat damage would be dealt to you", "damage would be dealt to this creature while it has a +1/+1 counter on it" (with a standing state that must hold too).</summary>
/// <exampledoc>Phantom Centaur</exampledoc>
/// <examplecapture>damage would be dealt to this creature</examplecapture>
[Dependent]
public class DamageWouldBeDealt : Glyph
{
    public override Nib[] Nibs => [Prop(Combat), "damage would be dealt to", Prop(Recipient), Prop(While)];

    [RegexPattern("combat")]
    public bool Combat { get; set; }
    public Recipient Recipient { get; set; }
    [Optional]
    public WhileHasCounters While { get; set; }
}
