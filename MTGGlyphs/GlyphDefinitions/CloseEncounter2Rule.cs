namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Close Encounter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Close Encounter</exampledoc>
[TokenizationOrder(-1)]
public class CloseEncounter2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage equal to the power of the chosen creature or card to target creature"];
}
