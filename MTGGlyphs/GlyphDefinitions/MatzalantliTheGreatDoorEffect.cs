namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Matzalantli, the Great Door // The Core, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Matzalantli, the Great Door // The Core</exampledoc>
[Dependent]
public class MatzalantliTheGreatDoorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["artifact, battle, creature, enchantment, land, and planeswalker are permanent types"];
}
