namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Louisoix's Sacrifice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Louisoix's Sacrifice</exampledoc>
[TokenizationOrder(-1)]
public class LouisoixsSacrificeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target activated ability, triggered ability, or noncreature spell"];
}
