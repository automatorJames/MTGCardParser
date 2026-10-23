namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Revealing Wind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Revealing Wind</exampledoc>
[TokenizationOrder(-1)]
public class RevealingWindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may look at each face-down creature that's attacking or blocking"];
}
