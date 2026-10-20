namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a permanent enters the battlefield: "enters tapped", "enters prepared", "enters with three +1/+1 counters on it" (modern Oracle wording drops "the battlefield").</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>enters with three +1/+1 counters on it</examplecapture>
[Dependent]
public class EntersTheBattlefield : Glyph, IPredicate
{
    public override Nib[] Nibs => ["enters", Prop(Tapped), Prop(Prepared), Prop(With)];

    [RegexPattern("tapped")]
    public bool Tapped { get; set; }
    [RegexPattern("prepared")]
    public bool Prepared { get; set; }
    [Optional]
    public EntersWithCounters With { get; set; }
}
