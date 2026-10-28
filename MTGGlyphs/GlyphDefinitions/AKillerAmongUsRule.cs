namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of A Killer Among Us, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>A Killer Among Us</exampledoc>
[TokenizationOrder(-1)]
public class AKillerAmongUsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then secretly choose human, merfolk, or goblin"];
}
