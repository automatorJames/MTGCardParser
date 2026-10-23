namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dragonhawk, Fate's Tempest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dragonhawk, Fate's Tempest</exampledoc>
[Dependent]
public class DragonhawkFatesTempestEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["dragonhawk deals 2 damage to each opponent for each of those cards that are still exiled"];
}
