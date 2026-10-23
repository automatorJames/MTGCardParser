namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Inquisitive Glimmer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inquisitive Glimmer</exampledoc>
[TokenizationOrder(-1)]
public class InquisitiveGlimmerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["unlock costs you pay cost {1} less"];
}
