namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tale's End, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tale's End</exampledoc>
[TokenizationOrder(-1)]
public class TalesEndRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target activated ability, triggered ability, or legendary spell"];
}
