namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Infectious Rage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infectious Rage</exampledoc>
[Dependent]
public class InfectiousRageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature at random this aura can enchant"];
}
