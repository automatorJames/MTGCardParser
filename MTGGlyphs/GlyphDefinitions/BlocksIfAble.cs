namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A forced block of a particular attacker, as a predicate: "target creature blocks this creature this turn if able", "have target creature … untap and block it if able" (provoke). Forced attacks and blocks with no particular object are AttacksIfAble.</summary>
/// <exampledoc>Rampant Elephant</exampledoc>
/// <examplecapture>blocks this creature this turn if able</examplecapture>
[Dependent]
public class BlocksIfAble : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(UntapsFirst), Alt("blocks", "block"), Prop(Attacker), Prop(Period), "if able"];

    [RegexPattern("untap and")]
    public bool UntapsFirst { get; set; }
    public PermanentPhrase Attacker { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
}
