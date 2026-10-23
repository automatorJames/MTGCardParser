namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rhystic Lightning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rhystic Lightning</exampledoc>
[TokenizationOrder(-1)]
public class RhysticLightningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 4 damage to any target unless that permanent's controller or that player pays {2}"];
}
