namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Moraug, Fury of Akoum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moraug, Fury of Akoum</exampledoc>
[TokenizationOrder(-1)]
public class MoraugFuryOfAkoumRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of that combat, untap all creatures you control"];
}
