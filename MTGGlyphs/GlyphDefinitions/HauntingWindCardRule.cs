namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Haunting Wind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haunting Wind</exampledoc>
[TokenizationOrder(-1)]
public class HauntingWindCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an artifact becomes tapped or a player activates an artifact's ability without {t} in its activation cost, this enchantment deals 1 damage to that artifact's controller"];
}
