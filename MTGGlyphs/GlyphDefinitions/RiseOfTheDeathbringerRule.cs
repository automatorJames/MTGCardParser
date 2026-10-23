namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rise of the Deathbringer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rise of the Deathbringer</exampledoc>
[TokenizationOrder(-1)]
public class RiseOfTheDeathbringerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you lose life equal to the number of cards drawn this way"];
}
