namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Flamehold Grappler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flamehold Grappler</exampledoc>
[Dependent]
public class FlameholdGrapplerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy the next spell you cast this turn when you cast it"];
}
