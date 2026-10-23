namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sigarda, Host of Herons, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sigarda, Host of Herons</exampledoc>
[TokenizationOrder(-1)]
public class SigardaHostOfHeronsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells and abilities your opponents control can't cause you to sacrifice permanents"];
}
