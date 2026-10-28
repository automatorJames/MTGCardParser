namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Squee's Revenge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Squee's Revenge</exampledoc>
[TokenizationOrder(-1)]
public class SqueesRevengeRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win all the flips, draw two cards for each flip"];
}
