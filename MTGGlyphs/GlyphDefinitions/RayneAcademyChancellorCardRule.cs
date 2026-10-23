namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rayne, Academy Chancellor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rayne, Academy Chancellor</exampledoc>
[TokenizationOrder(-1)]
public class RayneAcademyChancellorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you or a permanent you control becomes the target of a spell or ability an opponent controls, you may draw a card"];
}
