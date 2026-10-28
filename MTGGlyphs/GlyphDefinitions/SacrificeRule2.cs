namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sacrifice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sacrifice</exampledoc>
[TokenizationOrder(-1)]
public class SacrificeRule2 : Glyph, ICardSpecific
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["add an amount of {b} equal to the ", Nib.This, "d creature's mana value"];
}
