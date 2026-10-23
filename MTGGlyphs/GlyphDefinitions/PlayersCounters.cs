namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters a player has, as what "for each" counts: "poison counter your opponents have", "poison counter they have".</summary>
/// <exampledoc>Mycosynth Fiend</exampledoc>
/// <examplecapture>poison counter your opponents have</examplecapture>
[Dependent]
public class PlayersCounters : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "counter", Prop(Player), Pattern("ha(s|ve)")];

    public CounterType Kind { get; set; }
    public OneOf<WhichPlayer?, They> Player { get; set; }
}
