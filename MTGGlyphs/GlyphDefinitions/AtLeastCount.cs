namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"three or more", "two or more": a count at or above a number.</summary>
/// <exampledoc>Gorilla Berserkers</exampledoc>
/// <examplecapture>three or more</examplecapture>
[Dependent]
public class AtLeastCount : Glyph
{
    public override Nib[] Nibs => [Prop(Count), "or more"];

    public Quantity Count { get; set; }
}
