namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mechanized Production, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mechanized Production</exampledoc>
[TokenizationOrder(-1)]
public class MechanizedProductionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if you control eight or more artifacts with the same name as one another, you win the game"];
}
