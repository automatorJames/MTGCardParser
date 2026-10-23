namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sacrifice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sacrifice</exampledoc>
[TokenizationOrder(-1)]
public class Sacrifice2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell,", Nib.This, "a creature"];
}
