namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Several of a player's coming turns: "your next two turns", "their next x turns".</summary>
/// <exampledoc>Eater of Days</exampledoc>
/// <examplecapture>your next two turns</examplecapture>
[Dependent]
public class NextTurns : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), "next", Prop(Count), "turns"];

    public Whose Whose { get; set; }
    public Quantity Count { get; set; }
}
