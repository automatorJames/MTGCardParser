namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hidetsugu's Second Rite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hidetsugu's Second Rite</exampledoc>
[TokenizationOrder(-1)]
public class HidetsugusSecondRiteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if target player has exactly 10 life,", Nib.This, "deals 10 damage to that player"];
}
