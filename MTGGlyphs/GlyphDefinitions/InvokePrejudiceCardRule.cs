namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Invoke Prejudice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invoke Prejudice</exampledoc>
[TokenizationOrder(-1)]
public class InvokePrejudiceCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent casts a creature spell that doesn't share a color with a creature you control, counter that spell unless that player pays {x}, where x is its mana value"];
}
