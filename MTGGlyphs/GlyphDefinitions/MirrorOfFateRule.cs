namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirror of Fate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror of Fate</exampledoc>
[TokenizationOrder(-1)]
public class MirrorOfFateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all the cards from your library, then put the chosen cards on top of your library"];
}
