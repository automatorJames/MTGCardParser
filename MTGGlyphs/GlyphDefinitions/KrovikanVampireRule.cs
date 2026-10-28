namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Krovikan Vampire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Krovikan Vampire</exampledoc>
[TokenizationOrder(-1)]
public class KrovikanVampireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice it when you lose control of this creature"];
}
