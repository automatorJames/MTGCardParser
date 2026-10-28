namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Witchbane Orb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Witchbane Orb</exampledoc>
[Dependent]
public class WitchbaneOrbEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't be the target of spells or abilities your opponents control, including aura spells"];
}
