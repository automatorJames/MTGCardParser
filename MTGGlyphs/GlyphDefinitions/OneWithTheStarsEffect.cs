namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of One with the Stars, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>One with the Stars</exampledoc>
[Dependent]
public class OneWithTheStarsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it still has its abilities, but it's no longer a creature"];
}
