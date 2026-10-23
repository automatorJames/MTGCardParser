namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sigarda's Splendor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sigarda's Splendor</exampledoc>
/// <examplecapture>draw a card if your life total is greater than or equal to the last noted life total for this enchantment</examplecapture>
[Dependent]
public class SigardasSplendorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card if your life total is greater than or equal to the last noted life total for this enchantment"];
}
