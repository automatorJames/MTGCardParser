namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Moraug, Fury of Akoum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moraug, Fury of Akoum</exampledoc>
[TokenizationOrder(-1)]
public class MoraugFuryOfAkoumRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature you control gets +1/+0 for each time it has attacked this turn"];
}
