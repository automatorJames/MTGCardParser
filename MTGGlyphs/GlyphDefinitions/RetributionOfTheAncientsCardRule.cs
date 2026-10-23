namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Retribution of the Ancients, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Retribution of the Ancients</exampledoc>
[TokenizationOrder(-1)]
public class RetributionOfTheAncientsCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{b}, remove x +1/+1 counters from among creatures you control: target creature gets -x/-x until end of turn"];
}
