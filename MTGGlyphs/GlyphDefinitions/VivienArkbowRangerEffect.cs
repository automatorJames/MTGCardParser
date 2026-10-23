namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vivien, Arkbow Ranger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vivien, Arkbow Ranger</exampledoc>
[Dependent]
public class VivienArkbowRangerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["distribute two +1/+1 counters among up to two target creatures"];
}
