namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shapeshifter's Marrow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shapeshifter's Marrow</exampledoc>
[Dependent]
public class ShapeshiftersMarrowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it does, it loses this ability"];
}
