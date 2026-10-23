namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Super-Soldier Serum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Super-Soldier Serum</exampledoc>
[Dependent]
public class SuperSoldierSerumEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["attach any number of target equipment you control to it"];
}
