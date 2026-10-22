namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of copies of the token an earlier effect in the card created: "twelve of those tokens", "x of those tokens" - as a kicked or upgraded spell's bigger version ("if this spell was kicked, create twelve of those tokens instead").</summary>
/// <exampledoc>Conqueror's Pledge</exampledoc>
/// <examplecapture>twelve of those tokens</examplecapture>
[Dependent]
public class CountOfThoseTokens : Glyph
{
    public override Nib[] Nibs => [Prop(Count), "of those tokens"];

    public Quantity Count { get; set; }
}
