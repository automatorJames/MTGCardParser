namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Brutal Suppression, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brutal Suppression</exampledoc>
[TokenizationOrder(-1)]
public class BrutalSuppressionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activated abilities of nontoken rebels cost an additional \"sacrifice a land\" to activate"];
}
