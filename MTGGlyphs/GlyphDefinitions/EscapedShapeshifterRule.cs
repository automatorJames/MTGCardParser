namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Escaped Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Escaped Shapeshifter</exampledoc>
[TokenizationOrder(-1)]
public class EscapedShapeshifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as an opponent controls a creature with flying not named", Nib.This, ", this creature has flying"];
}
