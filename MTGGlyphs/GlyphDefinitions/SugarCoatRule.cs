namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sugar Coat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sugar Coat</exampledoc>
[TokenizationOrder(-1)]
public class SugarCoatRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted permanent is a colorless food artifact with \"{2}, {t}, sacrifice this artifact: you gain 3 life\" and loses all other card types and abilities"];
}
