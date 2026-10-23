namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Akroma, Angel of Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Akroma, Angel of Fury</exampledoc>
[TokenizationOrder(-1)]
public class AkromaAngelOfFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["turn her face up any time for her morph cost"];
}
