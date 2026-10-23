namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Irencrag, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Irencrag</exampledoc>
[TokenizationOrder(-1)]
public class TheIrencrag2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, it gains equip {3} and \"equipped creature gets +3/+3\" and loses all other abilities"];
}
