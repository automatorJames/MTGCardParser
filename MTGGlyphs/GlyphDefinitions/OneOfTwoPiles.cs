namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One of two piles (or of two cards) just separated or revealed: "one pile", "that pile", "one" (before "… and the other …").</summary>
/// <exampledoc>Fact or Fiction</exampledoc>
/// <examplecapture>one pile</examplecapture>
[Dependent]
public class OneOfTwoPiles : Glyph
{
    public override Nib[] Nibs => [Alt("one pile", "that pile", "one")];
}
