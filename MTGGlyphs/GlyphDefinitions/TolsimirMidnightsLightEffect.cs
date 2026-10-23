namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tolsimir, Midnight's Light, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tolsimir, Midnight's Light</exampledoc>
[Dependent]
public class TolsimirMidnightsLightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if tolsimir attacked this combat, target creature an opponent controls blocks that wolf this combat if able"];
}
