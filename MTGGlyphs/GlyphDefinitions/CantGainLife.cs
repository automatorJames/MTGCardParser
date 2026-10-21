namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's life total being locked against gains: "can't gain life", "can't gain life this turn", "can't gain life for the rest of the game".</summary>
/// <exampledoc>Erebos, God of the Dead</exampledoc>
/// <examplecapture>can't gain life</examplecapture>
[Dependent]
public class CantGainLife : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't gain life", Prop(Duration), Prop(RestOfGame)];

    [Optional]
    public OneOf<ThisPeriod, EffectDuration> Duration { get; set; }
    [RegexPattern("for the rest of the game")]
    public bool RestOfGame { get; set; }
}
