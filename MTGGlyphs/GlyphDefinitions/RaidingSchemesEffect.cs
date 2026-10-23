namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Raiding Schemes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raiding Schemes</exampledoc>
[Dependent]
public class RaidingSchemesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as you cast a noncreature spell, you may tap two untapped creatures you control that share a color with it"];
}
