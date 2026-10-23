namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Do or Die, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Do or Die</exampledoc>
[TokenizationOrder(-1)]
public class DoOrDieRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures in the pile of that player's choice"];
}
