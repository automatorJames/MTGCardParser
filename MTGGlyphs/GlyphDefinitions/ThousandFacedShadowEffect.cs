namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thousand-Faced Shadow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thousand-Faced Shadow</exampledoc>
[Dependent]
public class ThousandFacedShadowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's attacking, create a token that's a copy of another target attacking creature"];
}
