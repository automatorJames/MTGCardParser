namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Driftgloom Coyote, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Driftgloom Coyote</exampledoc>
[TokenizationOrder(-1)]
public class DriftgloomCoyoteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that creature had power 2 or less, put a +1/+1 counter on this creature"];
}
