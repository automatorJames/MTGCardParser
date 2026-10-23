namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Trinisphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trinisphere</exampledoc>
[TokenizationOrder(-1)]
public class TrinisphereRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this artifact is untapped, each spell that would cost less than three mana to cast costs three mana to cast"];
}
