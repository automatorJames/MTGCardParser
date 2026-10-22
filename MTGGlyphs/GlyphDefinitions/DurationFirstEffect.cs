namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect with its duration first: "until end of turn, you may play that card", "until the end of your next turn, you may play those cards", "until end of turn, target creature gains trample and gets +x/+0", "during your turn, this creature has first strike", "for the rest of the game, you can't cast spells" (epic).</summary>
/// <exampledoc>Alania's Pathmaker</exampledoc>
/// <examplecapture>until the end of your next turn, you may play that card</examplecapture>
public class DurationFirstEffect : Glyph
{
    public override Nib[] Nibs => [Prop(Duration), ",", Prop(Subject), Prop(Predicate)];

    public OneOf<UntilPhase, UntilEndOfPlayersTurn, OneOf<DuringTurn, ForTheRestOfTheGame>> Duration { get; set; }
    [Optional]
    public Subject Subject { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
