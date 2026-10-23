namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Energybending, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Energybending</exampledoc>
[TokenizationOrder(-1)]
public class EnergybendingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lands you control gain all basic land types until end of turn"];
}
