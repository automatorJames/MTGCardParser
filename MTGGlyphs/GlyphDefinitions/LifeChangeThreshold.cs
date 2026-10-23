namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's life having gone up or down by at least an amount: "you gained 4 or more life", "an opponent lost 2 or more life", "you've gained 3 or more life".</summary>
/// <exampledoc>Angelic Accord</exampledoc>
/// <examplecapture>you gained 4 or more life</examplecapture>
[Dependent]
public class LifeChangeThreshold : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Change), Prop(Amount), "or more life"];

    public OneOf<WhichPlayer?, PerfectSubject?> Player { get; set; }
    public LifeChange Change { get; set; }
    public Quantity Amount { get; set; }
}
