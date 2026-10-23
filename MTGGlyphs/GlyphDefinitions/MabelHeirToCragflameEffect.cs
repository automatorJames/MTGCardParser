namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mabel, Heir to Cragflame, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mabel, Heir to Cragflame</exampledoc>
[Dependent]
public class MabelHeirToCragflameEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create cragflame, a legendary colorless equipment artifact token with \"equipped creature gets +1/+1 and has vigilance, trample, and haste\" and equip {2}"];
}
