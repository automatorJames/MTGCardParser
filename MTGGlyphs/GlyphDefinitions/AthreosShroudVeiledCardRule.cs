namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Athreos, Shroud-Veiled, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Athreos, Shroud-Veiled</exampledoc>
[TokenizationOrder(-1)]
public class AthreosShroudVeiledCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature with a coin counter on it dies or is put into exile, return that card to the battlefield under your control"];
}
