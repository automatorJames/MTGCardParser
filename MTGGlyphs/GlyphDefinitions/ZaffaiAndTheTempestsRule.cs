namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zaffai and the Tempests, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zaffai and the Tempests</exampledoc>
[TokenizationOrder(-1)]
public class ZaffaiAndTheTempestsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once during each of your turns, you may cast an instant or sorcery spell from your hand without paying its mana cost"];
}
