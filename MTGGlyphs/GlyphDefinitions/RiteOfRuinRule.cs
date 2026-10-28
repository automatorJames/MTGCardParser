namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rite of Ruin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rite of Ruin</exampledoc>
[TokenizationOrder(-1)]
public class RiteOfRuinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose an order for artifacts, creatures, and lands"];
}
