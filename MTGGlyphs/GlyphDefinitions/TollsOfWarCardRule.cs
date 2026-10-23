namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tolls of War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tolls of War</exampledoc>
[TokenizationOrder(-1)]
public class TollsOfWarCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you sacrifice a permanent during your turn, create a 1/1 white ally creature token"];
}
