namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with three +1/+1 counters on it", "with a +1/+1 counter on it for each creature card in your graveyard", "with two +1/+1 counters on it and with flying", "with your choice of a reach counter or a vigilance counter on it", "with an additional +1/+1 counter on them", "with x +1/+1 counters on him" (a legend's pronoun): counters a permanent (or a group) enters the battlefield with, optionally multiplied, and optionally keywords alongside them.</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>with three +1/+1 counters on it</examplecapture>
[Dependent]
public class EntersWithCounters : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Counters), Alt("on it", "on them", "on him", "on her"), Prop(PerEach), Prop(AlsoWith)];

    public OneOf<Counters, ChoiceOfCounters> Counters { get; set; }
    [Optional]
    public ForEach PerEach { get; set; }
    [Optional]
    public AndEntersWithKeywords AlsoWith { get; set; }
}
