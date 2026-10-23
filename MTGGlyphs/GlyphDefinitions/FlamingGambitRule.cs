namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flaming Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flaming Gambit</exampledoc>
[TokenizationOrder(-1)]
public class FlamingGambitRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player or that planeswalker's controller may choose a creature they control and have", Nib.This, "deal that damage to it instead"];
}
