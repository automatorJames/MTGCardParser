namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lava Burst, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lava Burst</exampledoc>
[TokenizationOrder(-1)]
public class LavaBurstRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if", Nib.This, "would deal damage to a creature, that damage can't be prevented or dealt instead to another permanent or player"];
}
