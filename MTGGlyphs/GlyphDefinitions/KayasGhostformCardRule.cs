namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaya's Ghostform, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya's Ghostform</exampledoc>
[TokenizationOrder(-1)]
public class KayasGhostformCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when enchanted permanent dies or is put into exile, return that card to the battlefield under your control"];
}
