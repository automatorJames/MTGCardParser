namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you cast", "your opponents cast", "you cast from your graveyard", "you cast this turn", "your opponents cast during your turn", "you control": who casts (or controls) the spells meant, and optionally from where or when.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>you cast</examplecapture>
[Dependent]
public class SpellCaster : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Relation), Prop(From), Prop(ThisTurn), Prop(During)];

    public WhichPlayer Player { get; set; }
    public SpellRelation Relation { get; set; }
    [Optional]
    public CastFrom? From { get; set; }
    [Optional]
    [RegexPattern("this turn")]
    public bool? ThisTurn { get; set; }
    [Optional]
    public DuringTurn During { get; set; }
}
