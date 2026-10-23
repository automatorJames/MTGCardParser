namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Necromancer's Magemark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Necromancer's Magemark</exampledoc>
[TokenizationOrder(-1)]
public class NecromancersMagemarkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature you control that's enchanted would die, return it to its owner's hand instead"];
}
