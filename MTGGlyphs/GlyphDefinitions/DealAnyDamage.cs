namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Dealing damage with no recipient named, as a replacement's condition: "(would) deal combat damage", "(would) deal damage this turn".</summary>
/// <exampledoc>Inquisitor's Flail</exampledoc>
/// <examplecapture>deal combat damage</examplecapture>
[Dependent]
public class DealAnyDamage : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("deals?"), Prop(Combat), "damage", Prop(ThisTurn)];

    [Optional]
    [RegexPattern("combat")]
    public bool? Combat { get; set; }
    [Optional]
    [RegexPattern("this turn")]
    public bool? ThisTurn { get; set; }
}
