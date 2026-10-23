namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Endstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Endstone</exampledoc>
[Dependent]
public class TheEndstoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your life total becomes half your starting life total, rounded up"];
}
