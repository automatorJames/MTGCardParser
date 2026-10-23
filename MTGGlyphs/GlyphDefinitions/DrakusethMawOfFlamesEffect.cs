namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Drakuseth, Maw of Flames, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Drakuseth, Maw of Flames</exampledoc>
[Dependent]
public class DrakusethMawOfFlamesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["drakuseth deals 4 damage to any target and 3 damage to each of up to two other targets"];
}
