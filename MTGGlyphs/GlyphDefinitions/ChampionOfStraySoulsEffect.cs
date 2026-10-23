namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Champion of Stray Souls, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Champion of Stray Souls</exampledoc>
[Dependent]
public class ChampionOfStraySoulsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from your graveyard on top of your library"];
}
