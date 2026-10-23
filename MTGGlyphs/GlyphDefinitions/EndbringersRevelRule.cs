namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Endbringer's Revel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Endbringer's Revel</exampledoc>
/// <examplecapture>any player may activate this ability but only as a sorcery</examplecapture>
[TokenizationOrder(-1)]
public class EndbringersRevelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may activate this ability but only as a sorcery"];
}
