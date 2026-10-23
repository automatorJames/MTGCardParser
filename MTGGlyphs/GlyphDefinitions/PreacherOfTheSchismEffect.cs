namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Preacher of the Schism, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Preacher of the Schism</exampledoc>
[Dependent]
public class PreacherOfTheSchismEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you draw a card and you lose 1 life"];
}
