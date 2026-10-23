namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rebound, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rebound</exampledoc>
[TokenizationOrder(-1)]
public class ReboundCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the new target must be a player"];
}
