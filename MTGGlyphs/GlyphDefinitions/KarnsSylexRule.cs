namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Karn's Sylex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karn's Sylex</exampledoc>
[TokenizationOrder(-1)]
public class KarnsSylexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't pay life to cast spells or to activate abilities that aren't mana abilities"];
}
