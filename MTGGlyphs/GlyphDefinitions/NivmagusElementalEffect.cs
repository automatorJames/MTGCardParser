namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nivmagus Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nivmagus Elemental</exampledoc>
[Dependent]
public class NivmagusElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put two +1/+1 counters on this creature"];
}
