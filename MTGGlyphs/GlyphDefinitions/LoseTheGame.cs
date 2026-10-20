namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player winning or losing the game outright: "you lose the game", "you win the game", "that player loses the game".</summary>
/// <exampledoc>Lich</exampledoc>
/// <examplecapture>lose the game</examplecapture>
[Dependent]
public class LoseTheGame : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Result), "the game"];

    public GameResult Result { get; set; }
}
