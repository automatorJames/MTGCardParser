namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Risen Executioner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Risen Executioner</exampledoc>
[TokenizationOrder(-1)]
public class RisenExecutionerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this creature from your graveyard if you pay {1} more to cast it for each other creature card in your graveyard"];
}
