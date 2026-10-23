namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Verdant Mastery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Verdant Mastery</exampledoc>
[TokenizationOrder(-1)]
public class VerdantMasteryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put one of them onto the battlefield tapped under an opponent's control if the {3}{g} cost was paid"];
}
