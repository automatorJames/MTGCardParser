namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Heliod, the Radiant Dawn // Heliod, the Warped Eclipse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heliod, the Radiant Dawn // Heliod, the Warped Eclipse</exampledoc>
[Dependent]
public class HeliodTheRadiantDawnEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target enchantment card that isn't a god from your graveyard to your hand"];
}
