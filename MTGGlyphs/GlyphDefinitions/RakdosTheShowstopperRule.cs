namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rakdos, the Showstopper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos, the Showstopper</exampledoc>
[TokenizationOrder(-1)]
public class RakdosTheShowstopperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each creature whose coin comes up tails"];
}
