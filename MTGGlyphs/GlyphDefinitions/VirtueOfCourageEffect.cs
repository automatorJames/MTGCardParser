namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Virtue of Courage // Embereth Blaze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Virtue of Courage // Embereth Blaze</exampledoc>
[Dependent]
public class VirtueOfCourageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile that many cards from the top of your library"];
}
