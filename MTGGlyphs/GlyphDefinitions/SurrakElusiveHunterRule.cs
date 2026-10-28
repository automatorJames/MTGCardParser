namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Surrak, Elusive Hunter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Surrak, Elusive Hunter</exampledoc>
[TokenizationOrder(-1)]
public class SurrakElusiveHunterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control or a creature spell you control becomes the target of a spell or ability an opponent controls, draw a card"];
}
