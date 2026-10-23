namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Worms of the Earth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Worms of the Earth</exampledoc>
[Dependent]
public class WormsOfTheEarthEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may sacrifice two lands of their choice or have this enchantment deal 5 damage to that player"];
}
