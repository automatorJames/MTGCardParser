namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Wither's and infect's reminder rule: "[source] deals damage to creatures in the form of -1/-1 counters [and to players in the form of poison counters]" - the source may be "this", "it", "they", "creatures with infect".</summary>
/// <exampledoc>Boggart Ram-Gang</exampledoc>
/// <examplecapture>this deals damage to creatures in the form of -1/-1 counters</examplecapture>
[Dependent]
public class WitherRule : Glyph
{
    public override Nib[] Nibs => [Prop(Source), Pattern("deals?"), "damage to creatures in the form of -1/-1 counters", Prop(ToPlayers)];

    public OneOf<ThisCreature, Subject> Source { get; set; }
    [Optional]
    [RegexPattern("and to players in the form of poison counters")]
    public bool ToPlayers { get; set; }
}
