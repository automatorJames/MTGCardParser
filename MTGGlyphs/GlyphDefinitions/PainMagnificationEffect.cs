namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pain Magnification, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pain Magnification</exampledoc>
[Dependent]
public class PainMagnificationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player discards a card"];
}
