namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aegis Angel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aegis Angel</exampledoc>
[TokenizationOrder(-1)]
public class AegisAngelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a creature with indestructible can't be destroyed by damage"];
}
