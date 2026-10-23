namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dismantle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dismantle</exampledoc>
[TokenizationOrder(-1)]
public class DismantleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that artifact had counters on it, put that many +1/+1 counters or charge counters on an artifact you control"];
}
