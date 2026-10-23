namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Selvala, Eager Trailblazer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Selvala, Eager Trailblazer</exampledoc>
/// <examplecapture>add one mana of that color for each different power among creatures you control</examplecapture>
[TokenizationOrder(-1)]
public class SelvalaEagerTrailblazerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add one mana of that color for each different power among creatures you control"];
}
