namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balthier and Fran, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balthier and Fran</exampledoc>
[TokenizationOrder(-1)]
public class BalthierAndFranCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a vehicle crewed by", Nib.This, "this turn attacks, if it's the first combat phase of the turn, you may pay {1}{r}{g}"];
}
