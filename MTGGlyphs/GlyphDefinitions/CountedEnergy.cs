namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Energy as a number then one symbol: "eight {e}", "that many {e}", "any amount of {e}".</summary>
/// <exampledoc>Aethertide Whale</exampledoc>
/// <examplecapture>six {e}</examplecapture>
[Dependent]
public class CountedEnergy : Glyph
{
    public override Nib[] Nibs => [Prop(Count), "{e}"];

    public OneOf<Quantity?, ThatMany> Count { get; set; }
}
