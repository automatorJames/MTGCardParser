namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fated Firepower, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fated Firepower</exampledoc>
/// <examplecapture>if a source you control would deal damage to an opponent or a permanent an opponent controls, it deals that much damage plus an amount of damage equal to the number of fire counters on this enchantment instead</examplecapture>
[TokenizationOrder(-1)]
public class FatedFirepowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a source you control would deal damage to an opponent or a permanent an opponent controls, it deals that much damage plus an amount of damage equal to the number of fire counters on this enchantment instead"];
}
