namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spider-Woman, Secret Agent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Woman, Secret Agent</exampledoc>
[TokenizationOrder(-1)]
public class SpiderWomanSecretAgentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature can't become untapped for as long as you control spider-woman"];
}
