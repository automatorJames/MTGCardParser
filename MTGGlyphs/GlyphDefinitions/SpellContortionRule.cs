namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spell Contortion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spell Contortion</exampledoc>
[TokenizationOrder(-1)]
public class SpellContortionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each time", Nib.This, "was kicked"];
}
