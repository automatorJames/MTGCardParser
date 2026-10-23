namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player having paid something as an optional cost just described, as a condition: "you paid life this way", "you paid mana this way".</summary>
/// <exampledoc>Defiler of Vigor</exampledoc>
/// <examplecapture>you paid life this way</examplecapture>
[Dependent]
public class PaidThisWay : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "paid", Prop(Paid), "this way"];

    public WhichPlayer Player { get; set; }
    public PaidResource Paid { get; set; }
}
