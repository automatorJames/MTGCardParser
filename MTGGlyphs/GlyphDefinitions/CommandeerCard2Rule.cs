namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Commandeer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Commandeer</exampledoc>
[TokenizationOrder(-1)]
public class CommandeerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target noncreature spell"];
}
