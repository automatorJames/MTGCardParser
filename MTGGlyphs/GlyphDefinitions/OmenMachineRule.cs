namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Omen Machine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omen Machine</exampledoc>
[TokenizationOrder(-1)]
public class OmenMachineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, the player casts it without paying its mana cost if able"];
}
