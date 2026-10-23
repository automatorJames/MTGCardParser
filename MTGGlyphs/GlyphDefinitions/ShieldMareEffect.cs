namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shield Mare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shield Mare</exampledoc>
[Dependent]
public class ShieldMareEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 3 life"];
}
