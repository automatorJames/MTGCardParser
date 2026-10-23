namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Squee's Revenge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Squee's Revenge</exampledoc>
[TokenizationOrder(-1)]
public class SqueesRevengeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip a coin that many times or until you lose a flip, whichever comes first"];
}
