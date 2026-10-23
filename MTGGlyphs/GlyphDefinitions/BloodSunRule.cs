namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blood Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blood Sun</exampledoc>
[TokenizationOrder(-1)]
public class BloodSunRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all lands lose all abilities except mana abilities"];
}
