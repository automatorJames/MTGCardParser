namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dryad Arbor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dryad Arbor</exampledoc>
[Dependent]
public class DryadArborEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this land isn't a spell, it's affected by summoning sickness, and it has \"{t}: add {g}"];
}
