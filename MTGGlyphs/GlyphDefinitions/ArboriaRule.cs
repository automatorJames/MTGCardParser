namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Arboria, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arboria</exampledoc>
[TokenizationOrder(-1)]
public class ArboriaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures can't attack a player unless that player cast a spell or put a nontoken permanent onto the battlefield during their last turn"];
}
