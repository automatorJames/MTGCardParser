namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's life total being an exact number, as a condition: "you have exactly 1 life", "you have exactly 13 life".</summary>
/// <exampledoc>Near-Death Experience</exampledoc>
/// <examplecapture>you have exactly 1 life</examplecapture>
[Dependent]
public class LifeExactly : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Verb), "exactly", Prop(Life), "life"];

    public WhichPlayer Player { get; set; }
    public HaveVerb Verb { get; set; }
    public Quantity Life { get; set; }
}
