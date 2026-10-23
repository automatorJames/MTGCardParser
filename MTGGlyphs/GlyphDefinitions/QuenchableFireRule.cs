namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Quenchable Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quenchable Fire</exampledoc>
/// <examplecapture>it deals an additional 3 damage to that player or planeswalker at the beginning of your next upkeep step unless that player or that planeswalker's controller pays {u} before that step</examplecapture>
[TokenizationOrder(-1)]
public class QuenchableFireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals an additional 3 damage to that player or planeswalker at the beginning of your next upkeep step unless that player or that planeswalker's controller pays {u} before that step"];
}
