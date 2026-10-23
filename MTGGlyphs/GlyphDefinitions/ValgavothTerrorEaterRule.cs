namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Valgavoth, Terror Eater, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Valgavoth, Terror Eater</exampledoc>
[TokenizationOrder(-1)]
public class ValgavothTerrorEaterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a card you didn't control would be put into an opponent's graveyard from anywhere, exile it instead"];
}
