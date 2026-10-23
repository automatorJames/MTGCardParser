namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tezzeret, Cruel Captain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tezzeret, Cruel Captain</exampledoc>
[TokenizationOrder(-1)]
public class TezzeretCruelCaptainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's not a creature, it becomes a 0/0 robot artifact creature"];
}
