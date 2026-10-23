namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Swift Silence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swift Silence</exampledoc>
[TokenizationOrder(-1)]
public class SwiftSilence2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter all other spells"];
}
