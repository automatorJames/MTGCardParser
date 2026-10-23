namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sheriff of Safe Passage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sheriff of Safe Passage</exampledoc>
[TokenizationOrder(-1)]
public class SheriffOfSafePassageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature enters with a +1/+1 counter on it plus an additional +1/+1 counter on it for each other creature you control"];
}
