namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Summary Dismissal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Summary Dismissal</exampledoc>
[TokenizationOrder(-1)]
public class SummaryDismissalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all other spells and counter all abilities"];
}
