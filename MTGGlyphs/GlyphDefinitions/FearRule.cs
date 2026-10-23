namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fear, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fear</exampledoc>
[TokenizationOrder(-1)]
public class FearRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature has", Nib.This];
}
