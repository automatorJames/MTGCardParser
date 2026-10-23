namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Arni Metalbrow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arni Metalbrow</exampledoc>
[TokenizationOrder(-1)]
public class ArniMetalbrowCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control attacks or enters attacking, you may pay {1}{r}"];
}
