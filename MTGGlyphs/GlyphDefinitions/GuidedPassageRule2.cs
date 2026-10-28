namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guided Passage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guided Passage</exampledoc>
[TokenizationOrder(-1)]
public class GuidedPassageRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["an opponent chooses from among them a creature card, a land card, and a noncreature, nonland card"];
}
