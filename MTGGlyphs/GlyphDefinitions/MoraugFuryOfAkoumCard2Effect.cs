namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Moraug, Fury of Akoum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moraug, Fury of Akoum</exampledoc>
[Dependent]
public class MoraugFuryOfAkoumCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap all creatures you control"];
}
