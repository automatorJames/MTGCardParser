namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Well of Knowledge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Well of Knowledge</exampledoc>
[TokenizationOrder(-1)]
public class WellOfKnowledgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may activate this ability but only during their draw step"];
}
