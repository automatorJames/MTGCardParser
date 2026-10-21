namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Indestructible's second reminder sentence: "if its toughness is 0 or less, it still dies".</summary>
/// <exampledoc>Darksteel Myr</exampledoc>
/// <examplecapture>if its toughness is 0 or less, it still dies</examplecapture>
public class StillDiesAtZeroToughness : Glyph
{
    public override Nib[] Nibs => ["if its toughness is 0 or less, it still dies"];
}
