namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Harness Infinity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Harness Infinity</exampledoc>
[TokenizationOrder(-1)]
public class HarnessInfinityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange your hand and graveyard"];
}
