namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pendelhaven Elder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pendelhaven Elder</exampledoc>
[Dependent]
public class PendelhavenElderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each 1/1 creature you control gets +1/+2 until end of turn"];
}
