namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the other": the remaining one of two objects after one was picked out earlier in the effect ("exile one of those creatures and put two +1/+1 counters on the other").</summary>
/// <exampledoc>Cannibalize</exampledoc>
/// <examplecapture>the other</examplecapture>
[Dependent]
public class TheOther : Glyph
{
    public override Nib[] Nibs => ["the other"];
}
