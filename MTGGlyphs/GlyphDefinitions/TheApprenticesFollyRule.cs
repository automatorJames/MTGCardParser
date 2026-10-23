namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Apprentice's Folly, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Apprentice's Folly</exampledoc>
[TokenizationOrder(-1)]
public class TheApprenticesFollyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["i, ii — choose target nontoken creature you control that doesn't have the same name as a token you control"];
}
