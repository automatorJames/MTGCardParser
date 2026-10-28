namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bronze Bombshell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bronze Bombshell</exampledoc>
[TokenizationOrder(-1)]
public class BronzeBombshellRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when a player other than this creature's owner controls it, that player sacrifices it"];
}
