namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Builder's Bane, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Builder's Bane</exampledoc>
[TokenizationOrder(-1)]
public class BuildersBaneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage to each player equal to the number of artifacts they controlled that were put into a graveyard this way"];
}
