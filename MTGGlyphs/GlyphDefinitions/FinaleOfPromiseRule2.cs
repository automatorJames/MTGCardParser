namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Finale of Promise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Finale of Promise</exampledoc>
[TokenizationOrder(-1)]
public class FinaleOfPromiseRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 10 or more, copy each of those spells twice"];
}
