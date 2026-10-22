namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The piles cards or permanents are separated into: "two piles", "three piles", "two face-down piles", "a face-down pile and a face-up pile".</summary>
/// <exampledoc>Fact or Fiction</exampledoc>
/// <examplecapture>two piles</examplecapture>
[Dependent]
public class Piles : Glyph
{
    public override Nib[] Nibs => [Pattern("((two|three) (face-down )?piles|a face-down pile and a face-up pile)")];
}
