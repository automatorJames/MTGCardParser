namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"only if" a state of the game holds, as an activation restriction: "activate only if you control three or more artifacts", "… if you have no cards in hand", "… if this card is in your graveyard". Uses the shared GameState, so restrictions read like static abilities' conditions.</summary>
/// <exampledoc>Mox Opal</exampledoc>
/// <examplecapture>if you control three or more artifacts</examplecapture>
[Dependent]
public class IfGameState : Glyph
{
    public override Nib[] Nibs => [Opt("only"), "if", Prop(State)];

    public OneOf<GameState, CardIsInZone> State { get; set; }
}
