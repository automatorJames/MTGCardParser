namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ethereal Ambush, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ethereal Ambush</exampledoc>
[TokenizationOrder(-1)]
public class EtherealAmbushRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["manifest the top two cards of your library"];
}
