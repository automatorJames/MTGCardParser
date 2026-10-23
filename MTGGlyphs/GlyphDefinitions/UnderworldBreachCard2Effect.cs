namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Underworld Breach, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Underworld Breach</exampledoc>
[Dependent]
public class UnderworldBreachCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast cards from your graveyard for their escape cost"];
}
