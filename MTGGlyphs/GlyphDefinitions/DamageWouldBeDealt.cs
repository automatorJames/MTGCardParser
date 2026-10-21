namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage about to be dealt, as a replacement condition: "damage would be dealt to this creature", "combat damage would be dealt to you".</summary>
/// <exampledoc>Phantom Centaur</exampledoc>
/// <examplecapture>damage would be dealt to this creature</examplecapture>
[Dependent]
public class DamageWouldBeDealt : Glyph
{
    public override Nib[] Nibs => [Prop(Combat), "damage would be dealt to", Prop(Recipient)];

    [RegexPattern("combat")]
    public bool Combat { get; set; }
    public Recipient Recipient { get; set; }
}
