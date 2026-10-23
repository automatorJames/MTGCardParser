namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Preacher of the Schism, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Preacher of the Schism</exampledoc>
[Dependent]
public class PreacherOfTheSchism2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 white vampire creature token with lifelink"];
}
