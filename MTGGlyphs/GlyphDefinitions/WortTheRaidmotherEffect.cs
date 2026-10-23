namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wort, the Raidmother, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wort, the Raidmother</exampledoc>
[Dependent]
public class WortTheRaidmotherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as you cast the spell, you may tap two untapped creatures you control that share a color with it"];
}
