namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Disorder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Disorder</exampledoc>
[TokenizationOrder(-1)]
public class DisorderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 2 damage to each white creature and each player who controls a white creature"];
}
