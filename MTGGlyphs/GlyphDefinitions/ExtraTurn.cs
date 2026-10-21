namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Taking an extra turn: "take an extra turn after this one", "target player takes an extra turn after this one", "that player takes an extra turn".</summary>
/// <exampledoc>Time Warp</exampledoc>
/// <examplecapture>takes an extra turn after this one</examplecapture>
public class ExtraTurn : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("takes?"), "an extra turn", Prop(AfterThisOne)];

    [RegexPattern("after this one")]
    public bool AfterThisOne { get; set; }
}
