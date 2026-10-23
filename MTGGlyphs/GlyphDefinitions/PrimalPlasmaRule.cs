namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Primal Plasma, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Primal Plasma</exampledoc>
[TokenizationOrder(-1)]
public class PrimalPlasmaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this creature enters, it becomes your choice of a 3/3 creature, a 2/2 creature with flying, or a 1/6 creature with defender"];
}
