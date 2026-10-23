namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Calix, Destiny's Hand, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Calix, Destiny's Hand</exampledoc>
[Dependent]
public class CalixDestinysHandEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target creature or enchantment you don't control until target enchantment you control leaves the battlefield"];
}
