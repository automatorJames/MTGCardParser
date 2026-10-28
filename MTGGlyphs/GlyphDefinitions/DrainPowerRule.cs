namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Drain Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Drain Power</exampledoc>
[TokenizationOrder(-1)]
public class DrainPowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player activates a mana ability of each land they control"];
}
