namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Arcanum Wings, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arcanum Wings</exampledoc>
[Dependent]
public class ArcanumWingsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange this aura with an aura card in your hand"];
}
