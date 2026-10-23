namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kothophed, Soul Hoarder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kothophed, Soul Hoarder</exampledoc>
[TokenizationOrder(-1)]
public class KothophedSoulHoarderCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a permanent owned by another player is put into a graveyard from the battlefield, you draw a card and you lose 1 life"];
}
