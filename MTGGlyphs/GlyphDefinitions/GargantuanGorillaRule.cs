namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gargantuan Gorilla, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gargantuan Gorilla</exampledoc>
[TokenizationOrder(-1)]
public class GargantuanGorillaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't sacrifice a forest, sacrifice this creature and it deals 7 damage to you"];
}
