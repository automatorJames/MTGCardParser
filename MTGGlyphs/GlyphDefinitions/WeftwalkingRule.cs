namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Weftwalking, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Weftwalking</exampledoc>
[TokenizationOrder(-1)]
public class WeftwalkingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the first spell each player casts during each of their turns may be cast without paying its mana cost"];
}
