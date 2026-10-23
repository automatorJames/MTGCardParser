namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Flameskull, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flameskull</exampledoc>
[Dependent]
public class FlameskullEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast", Nib.This, "this way, you can't play the other card, and vice versa"];
}
