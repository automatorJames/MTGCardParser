namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Meteor Crater, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meteor Crater</exampledoc>
[Dependent]
public class MeteorCraterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a color of a permanent you control"];
}
