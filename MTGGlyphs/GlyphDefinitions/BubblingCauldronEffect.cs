namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bubbling Cauldron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bubbling Cauldron</exampledoc>
[Dependent]
public class BubblingCauldronEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent loses 4 life"];
}
