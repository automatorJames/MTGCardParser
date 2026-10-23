namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Steer Clear, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Steer Clear</exampledoc>
[TokenizationOrder(-1)]
public class SteerClearRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 4 damage to that creature instead if you controlled a mount as you cast this spell"];
}
