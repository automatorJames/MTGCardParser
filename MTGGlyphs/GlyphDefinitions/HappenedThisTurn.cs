namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something having happened earlier this turn or last turn, as a condition: "you attacked this turn" (raid), "a creature died this turn" (morbid), "a permanent left the battlefield under your control this turn" (revolt), "an opponent lost life last turn", "you gained 4 or more life this turn", "you didn't play a land this turn", "you drew two or more cards this turn".</summary>
/// <exampledoc>Mardu Heart-Piercer</exampledoc>
/// <examplecapture>you attacked this turn</examplecapture>
[Dependent]
public class HappenedThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Event), Prop(Turn)];

    public OneOf<TurnEvent?, LifeChangeThreshold, PlayerActionCount> Event { get; set; }
    public TurnSpan Turn { get; set; }
}
