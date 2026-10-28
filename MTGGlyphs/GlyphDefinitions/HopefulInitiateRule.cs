namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hopeful Initiate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hopeful Initiate</exampledoc>
[TokenizationOrder(-1)]
public class HopefulInitiateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{2}{w}, remove two +1/+1 counters from among creatures you control: destroy target artifact or enchantment"];
}
