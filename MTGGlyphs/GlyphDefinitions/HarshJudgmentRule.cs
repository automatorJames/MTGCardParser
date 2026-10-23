namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Harsh Judgment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Harsh Judgment</exampledoc>
[TokenizationOrder(-1)]
public class HarshJudgmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an instant or sorcery spell of the chosen color would deal damage to you, it deals that damage to its controller instead"];
}
