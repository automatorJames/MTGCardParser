namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Death or Glory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Death or Glory</exampledoc>
[TokenizationOrder(-1)]
public class DeathOrGloryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the pile of an opponent's choice and return the other to the battlefield"];
}
