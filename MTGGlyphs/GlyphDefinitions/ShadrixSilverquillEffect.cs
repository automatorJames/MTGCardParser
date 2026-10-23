namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shadrix Silverquill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shadrix Silverquill</exampledoc>
[Dependent]
public class ShadrixSilverquillEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may choose two"];
}
