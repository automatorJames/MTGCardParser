namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rasputin Dreamweaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rasputin Dreamweaver</exampledoc>
[Dependent]
public class RasputinDreamweaverEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent the next 1 damage that would be dealt to rasputin this turn"];
}
