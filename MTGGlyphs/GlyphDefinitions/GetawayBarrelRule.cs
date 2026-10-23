namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Getaway Barrel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Getaway Barrel</exampledoc>
[TokenizationOrder(-1)]
public class GetawayBarrelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a random creature card from among them onto the battlefield"];
}
