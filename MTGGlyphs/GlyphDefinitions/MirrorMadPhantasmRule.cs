namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirror-Mad Phantasm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror-Mad Phantasm</exampledoc>
[TokenizationOrder(-1)]
public class MirrorMadPhantasmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that player does, they reveal cards from the top of that library until a card named", Nib.This, "is revealed"];
}
