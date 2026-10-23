namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mutinous Massacre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mutinous Massacre</exampledoc>
[TokenizationOrder(-1)]
public class MutinousMassacreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each creature with mana value of the chosen quality"];
}
