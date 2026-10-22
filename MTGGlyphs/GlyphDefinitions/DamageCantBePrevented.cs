namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A rule that damage can't be prevented: "damage can't be prevented", "the damage can't be prevented", "combat damage can't be prevented", "damage can't be prevented this turn", "damage that would be dealt by this creature can't be prevented".</summary>
/// <exampledoc>Everlasting Torment</exampledoc>
/// <examplecapture>damage can't be prevented</examplecapture>
public class DamageCantBePrevented : Glyph
{
    public override Nib[] Nibs => [Prop(Which), Prop(Combat), "damage", Prop(Dealt), "can't be prevented", Prop(Turn)];

    [Optional]
    public DamageDeterminer? Which { get; set; }
    [Optional]
    [RegexPattern("combat")]
    public bool Combat { get; set; }
    [Optional]
    public WouldBeDealtBy Dealt { get; set; }
    [Optional]
    public TurnSpan? Turn { get; set; }
}
