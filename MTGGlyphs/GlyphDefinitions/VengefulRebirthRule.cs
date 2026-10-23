namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vengeful Rebirth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vengeful Rebirth</exampledoc>
[TokenizationOrder(-1)]
public class VengefulRebirthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you return a nonland card to your hand this way,", Nib.This, "deals damage equal to that card's mana value to any target"];
}
