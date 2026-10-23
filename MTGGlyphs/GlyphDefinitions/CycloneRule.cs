namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cyclone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cyclone</exampledoc>
[TokenizationOrder(-1)]
public class CycloneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you pay, this enchantment deals damage equal to the number of wind counters on it to each creature and each player"];
}
