namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a permanent enters the battlefield: "enters the battlefield tapped", "enters the battlefield with three +1/+1 counters on it".</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>enters the battlefield with three +1/+1 counters on it</examplecapture>
[Dependent]
public class EntersTheBattlefield : Glyph, IPredicate
{
    public override Nib[] Nibs => ["enters the battlefield", Prop(Tapped), Prop(With)];

    [RegexPattern("tapped")]
    public bool Tapped { get; set; }
    [Optional]
    public EntersWithCounters With { get; set; }
}
