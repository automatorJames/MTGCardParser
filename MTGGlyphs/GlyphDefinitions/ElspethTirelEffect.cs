namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Elspeth Tirel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elspeth Tirel</exampledoc>
[Dependent]
public class ElspethTirelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all other permanents except for lands and tokens"];
}
