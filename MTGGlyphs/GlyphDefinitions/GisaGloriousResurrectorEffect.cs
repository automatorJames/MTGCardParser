namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gisa, Glorious Resurrector, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gisa, Glorious Resurrector</exampledoc>
[Dependent]
public class GisaGloriousResurrectorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put all creature cards exiled with gisa onto the battlefield under your control"];
}
