namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Taking over combat decisions: "choose which creatures attack this turn", "choose which creatures block this combat and how those creatures block". A predicate, so its chooser is the subject ("you choose …").</summary>
/// <exampledoc>Master Warcraft</exampledoc>
/// <examplecapture>choose which creatures block this turn and how those creatures block</examplecapture>
[Dependent]
public class ChooseCombatants : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("chooses?"), "which creatures", Prop(Action), Alt("this turn", "this combat"), Prop(HowTheyBlock)];

    public CombatAction Action { get; set; }
    [Optional]
    [RegexPattern("and how those creatures block")]
    public bool HowTheyBlock { get; set; }
}
