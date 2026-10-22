namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"roll a d20", "roll two d6": a random numeric outcome, read by the result table rows that follow (see <see cref="DieResultRow"/>). A predicate, so triggers, costs' effects and "then roll a d20" resolve it.</summary>
/// <exampledoc>Djinni Windseer</exampledoc>
/// <examplecapture>roll a d20</examplecapture>
public class RollDie : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["roll", Prop(Count), Prop(Die)];

    public Quantity Count { get; set; }
    public DieSize Die { get; set; }
}
