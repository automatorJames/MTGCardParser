namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ice Cave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ice Cave</exampledoc>
[Dependent]
public class IceCaveEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any other player may pay that spell's mana cost"];
}
