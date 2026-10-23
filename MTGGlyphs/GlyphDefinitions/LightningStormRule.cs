namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lightning Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lightning Storm</exampledoc>
[TokenizationOrder(-1)]
public class LightningStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may activate this ability but only if", Nib.This, "is on the stack"];
}
