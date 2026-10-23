namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nissa's Pilgrimage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nissa's Pilgrimage</exampledoc>
/// <examplecapture>spell mastery — if there are two or more instant and/or sorcery cards in your graveyard, search your library for up to three basic forest cards instead of two</examplecapture>
[TokenizationOrder(-1)]
public class NissasPilgrimageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spell mastery — if there are two or more instant and/or sorcery cards in your graveyard, search your library for up to three basic forest cards instead of two"];
}
