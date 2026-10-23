namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mindbreak Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindbreak Trap</exampledoc>
[TokenizationOrder(-1)]
public class MindbreakTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile any number of target spells"];
}
