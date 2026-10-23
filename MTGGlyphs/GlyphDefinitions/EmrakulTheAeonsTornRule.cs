namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Emrakul, the Aeons Torn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emrakul, the Aeons Torn</exampledoc>
[TokenizationOrder(-1)]
public class EmrakulTheAeonsTornRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flying, protection from spells that are one or more colors, annihilator 6"];
}
