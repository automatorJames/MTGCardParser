namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nephalia Academy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nephalia Academy</exampledoc>
/// <examplecapture>if a spell or ability an opponent controls causes you to discard a card, you may reveal that card and put it on top of your library instead of putting it anywhere else</examplecapture>
[TokenizationOrder(-1)]
public class NephaliaAcademyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a spell or ability an opponent controls causes you to discard a card, you may reveal that card and put it on top of your library instead of putting it anywhere else"];
}
