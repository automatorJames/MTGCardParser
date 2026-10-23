namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zariel, Archduke of Avernus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zariel, Archduke of Avernus</exampledoc>
[Dependent]
public class ZarielArchdukeOfAvernusEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"at the end of the first combat phase on your turn, untap target creature you control"];
}
