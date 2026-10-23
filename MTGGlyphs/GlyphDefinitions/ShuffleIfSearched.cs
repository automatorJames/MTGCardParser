namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The shuffle that follows a search of several zones including the library: "if you search your library this way, shuffle". A stock sentence; ordered first so the general "if … this way, …" reading (<see cref="ActedOnThisWay"/>) doesn't take it.</summary>
/// <exampledoc>Ajani's Aid</exampledoc>
/// <examplecapture>if you search your library this way, shuffle</examplecapture>
[TokenizationOrder(0)]
public class ShuffleIfSearched : Glyph
{
    public override Nib[] Nibs => ["if you search your library this way, shuffle"];
}
