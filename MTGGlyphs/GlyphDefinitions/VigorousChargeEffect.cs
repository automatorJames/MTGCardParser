namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vigorous Charge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vigorous Charge</exampledoc>
[Dependent]
public class VigorousChargeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was kicked, you gain life equal to that damage"];
}
