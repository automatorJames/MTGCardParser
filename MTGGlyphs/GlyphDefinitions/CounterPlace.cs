namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where counters go or come from: "on target creature", "from {this}", and for suspend effects also exiled suspended cards: "from target permanent or suspended card", "on each permanent with a time counter on it and each suspended card".</summary>
/// <exampledoc>Armageddon Clock</exampledoc>
/// <examplecapture>on {this}</examplecapture>
[Dependent]
public class CounterPlace : Glyph
{
    public override Nib[] Nibs => [Alt("on", "from"), Prop(Permanent), Prop(OrSuspendedCard)];

    public CounterRecipient Permanent { get; set; }
    [RegexPattern("(?:or|and each) suspended card(?: you own)?")]
    public bool OrSuspendedCard { get; set; }
}
