namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A rule that damage can't be prevented: "damage can't be prevented", "the damage can't be prevented", "combat damage can't be prevented", "damage can't be prevented this turn", "damage that would be dealt by this creature can't be prevented". The determiner and "combat" are one optional enum, since an optional enum followed by an optional bool never matched.</summary>
/// <exampledoc>Everlasting Torment</exampledoc>
/// <examplecapture>damage can't be prevented</examplecapture>
public class DamageCantBePrevented : Glyph
{
    public override Nib[] Nibs => [Prop(Which), "damage", Prop(Dealt), "can't be prevented", Prop(Turn)];

    [Optional]
    public DamageScope? Which { get; set; }
    [Optional]
    public WouldBeDealtBy Dealt { get; set; }
    [Optional]
    public TurnSpan? Turn { get; set; }
}
