namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bookwurm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bookwurm</exampledoc>
[Dependent]
public class BookwurmEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from your graveyard into your library third from the top"];
}
