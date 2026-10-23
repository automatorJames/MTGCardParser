namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rampaging Geoderm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rampaging Geoderm</exampledoc>
[TokenizationOrder(-1)]
public class RampagingGeodermRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's attacking a battle, put a +1/+1 counter on it instead"];
}
