namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blind Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blind Fury</exampledoc>
[TokenizationOrder(-1)]
public class BlindFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature would deal combat damage to a creature this turn, it deals double that damage to that creature instead"];
}
