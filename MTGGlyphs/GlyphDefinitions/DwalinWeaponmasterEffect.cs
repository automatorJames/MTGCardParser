namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dwalin, Weaponmaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dwalin, Weaponmaster</exampledoc>
[Dependent]
public class DwalinWeaponmasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each hone counter on an equipment grants +1/+0 to equipped creature"];
}
