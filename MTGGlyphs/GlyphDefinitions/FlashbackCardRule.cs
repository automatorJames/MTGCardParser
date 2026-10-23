namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flashback, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flashback</exampledoc>
[TokenizationOrder(-1)]
public class FlashbackCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target instant or sorcery card in your graveyard gains", Nib.This, "until end of turn"];
}
