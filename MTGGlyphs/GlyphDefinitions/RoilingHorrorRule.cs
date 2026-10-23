namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Roiling Horror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roiling Horror</exampledoc>
[TokenizationOrder(-1)]
public class RoilingHorrorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "'s power and toughness are each equal to your life total minus the life total of an opponent with the most life"];
}
