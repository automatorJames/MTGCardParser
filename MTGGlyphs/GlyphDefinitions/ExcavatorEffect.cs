namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Excavator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Excavator</exampledoc>
[Dependent]
public class ExcavatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature gains landwalk of each of the land types of the sacrificed land until end of turn"];
}
