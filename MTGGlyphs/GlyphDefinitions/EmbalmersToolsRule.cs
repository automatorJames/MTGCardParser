namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Embalmer's Tools, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Embalmer's Tools</exampledoc>
[TokenizationOrder(-1)]
public class EmbalmersToolsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activated abilities of creature cards in your graveyard cost {1} less to activate"];
}
