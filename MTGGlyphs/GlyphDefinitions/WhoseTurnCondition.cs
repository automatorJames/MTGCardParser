namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose turn (or phase of a turn) it is, as a condition: "it's your turn", "it's not your turn", "it's an opponent's turn", "it's not their turn", "it isn't your main phase", "it's your combat phase".</summary>
/// <exampledoc>Fated Conflagration</exampledoc>
/// <examplecapture>it's your turn</examplecapture>
[Dependent]
public class WhoseTurnCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Copula), Prop(Whose), Prop(Phase)];

    public TurnCopula Copula { get; set; }
    public Whose Whose { get; set; }
    public Phase Phase { get; set; }
}
