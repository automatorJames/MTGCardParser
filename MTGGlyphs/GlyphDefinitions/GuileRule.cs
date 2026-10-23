namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guile, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guile</exampledoc>
/// <examplecapture>if a spell or ability you control would counter a spell, instead exile that spell and you may play that card without paying its mana cost</examplecapture>
[TokenizationOrder(-1)]
public class GuileRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a spell or ability you control would counter a spell, instead exile that spell and you may play that card without paying its mana cost"];
}
