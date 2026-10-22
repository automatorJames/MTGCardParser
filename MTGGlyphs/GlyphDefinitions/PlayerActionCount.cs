namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player having done something a number of times, as a condition: "you drew two or more cards", "a player cast two or more spells", "you sacrificed three or more clues", "you've cast three or more instant and sorcery spells".</summary>
/// <exampledoc>Archmage Ascension</exampledoc>
/// <examplecapture>you drew two or more cards</examplecapture>
[Dependent]
public class PlayerActionCount : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Action), Prop(Amount), "or more", Prop(What)];

    public OneOf<PerfectSubject?, WhichPlayer?> Player { get; set; }
    public PlayerTurnAction Action { get; set; }
    public Quantity Amount { get; set; }
    public OneOf<SpellsCast, PluralCards, Permanents> What { get; set; }
}
