namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something having happened earlier this turn, as a condition: "you attacked this turn" (raid), "a creature died this turn" (morbid), "a permanent left the battlefield under your control this turn" (revolt), "an opponent lost life this turn", "you gained life this turn", "you've cast another spell this turn".</summary>
/// <exampledoc>Mardu Heart-Piercer</exampledoc>
/// <examplecapture>you attacked this turn</examplecapture>
[Dependent]
public class HappenedThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Event), "this turn"];

    public TurnEvent Event { get; set; }
}
