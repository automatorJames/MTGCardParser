namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Intet, the Dreamer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Intet, the Dreamer</exampledoc>
[TokenizationOrder(-1)]
public class IntetTheDreamerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play that card without paying its mana cost for as long as intet remains on the battlefield"];
}
