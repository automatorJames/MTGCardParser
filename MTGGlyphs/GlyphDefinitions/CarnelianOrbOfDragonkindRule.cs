namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Carnelian Orb of Dragonkind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Carnelian Orb of Dragonkind</exampledoc>
[TokenizationOrder(-1)]
public class CarnelianOrbOfDragonkindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that mana is spent on a dragon creature spell, it gains haste until end of turn"];
}
