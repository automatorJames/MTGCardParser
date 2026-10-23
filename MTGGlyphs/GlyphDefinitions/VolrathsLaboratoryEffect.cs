namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Volrath's Laboratory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volrath's Laboratory</exampledoc>
[Dependent]
public class VolrathsLaboratoryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 2/2 creature token of the chosen color and type"];
}
