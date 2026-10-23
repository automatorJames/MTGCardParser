namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hazoret's Undying Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hazoret's Undying Fury</exampledoc>
[TokenizationOrder(-1)]
public class HazoretsUndyingFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle your library, then exile the top four cards"];
}
