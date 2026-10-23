namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lunar Rejection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lunar Rejection</exampledoc>
[TokenizationOrder(-1)]
public class LunarRejectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target [wolf or werewolf] creature to its owner's hand"];
}
