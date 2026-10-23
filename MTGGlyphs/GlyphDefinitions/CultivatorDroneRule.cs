namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cultivator Drone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cultivator Drone</exampledoc>
[TokenizationOrder(-1)]
public class CultivatorDroneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast a colorless spell, activate an ability of a colorless permanent, or pay a cost that contains {c}"];
}
