namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"searched their library this way": a player who took up an offer to search.</summary>
/// <exampledoc>Hired Giant</exampledoc>
/// <examplecapture>searched their library this way</examplecapture>
[Dependent]
public class SearchedThisWay : Glyph
{
    public override Nib[] Nibs => ["searched their library this way"];
}
