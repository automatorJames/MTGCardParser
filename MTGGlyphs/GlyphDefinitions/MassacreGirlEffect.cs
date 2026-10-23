namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Massacre Girl, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Massacre Girl</exampledoc>
[Dependent]
public class MassacreGirlEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature other than", Nib.This, "gets -1/-1 until end of turn"];
}
