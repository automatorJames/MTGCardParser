namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lava Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lava Storm</exampledoc>
[TokenizationOrder(-1)]
public class LavaStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 2 damage to each attacking creature or", Nib.This, "deals 2 damage to each blocking creature"];
}
