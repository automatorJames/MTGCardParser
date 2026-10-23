namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Retribution of the Ancients, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Retribution of the Ancients</exampledoc>
[Dependent]
public class RetributionOfTheAncientsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature gets -x/-x until end of turn"];
}
