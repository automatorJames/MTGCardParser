namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Karmic Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karmic Justice</exampledoc>
[TokenizationOrder(-1)]
public class KarmicJusticeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a spell or ability an opponent controls destroys a noncreature permanent you control, you may destroy target permanent that opponent controls"];
}
