namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Backdraft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Backdraft</exampledoc>
[TokenizationOrder(-1)]
public class BackdraftRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage to that player equal to half the damage dealt by one of those sorcery spells this turn, rounded down"];
}
