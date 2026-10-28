namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's life total having changed earlier in the turn, said of players picked out by "who": "lost life this turn", "gained life this turn".</summary>
/// <exampledoc>Teysa, Opulent Oligarch</exampledoc>
/// <examplecapture>lost life this turn</examplecapture>
[Dependent]
public class LifeChangedThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Change), "life", Prop(Turn)];

    public LifeChange Change { get; set; }
    public TurnSpan Turn { get; set; }
}
