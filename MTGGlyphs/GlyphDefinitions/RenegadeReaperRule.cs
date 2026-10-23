namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Renegade Reaper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Renegade Reaper</exampledoc>
[TokenizationOrder(-1)]
public class RenegadeReaperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if at least one angel card is milled this way, you gain 4 life"];
}
