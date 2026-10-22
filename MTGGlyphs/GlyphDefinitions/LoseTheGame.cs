namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player winning or losing the game outright, or being kept from it: "you lose the game", "you win the game", "that player loses the game", "you can't lose the game this turn", "your opponents can't win the game", "you don't lose the game for having 0 or less life".</summary>
/// <exampledoc>Lich</exampledoc>
/// <examplecapture>don't lose the game for having 0 or less life</examplecapture>
[Dependent]
public class LoseTheGame : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Negation), Prop(Result), "the game", Prop(ThisTurn), Prop(ForZeroLife)];

    [Optional]
    public Negation? Negation { get; set; }
    public GameResult Result { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurn { get; set; }
    [RegexPattern("for having 0 or less life")]
    public bool ForZeroLife { get; set; }
}
