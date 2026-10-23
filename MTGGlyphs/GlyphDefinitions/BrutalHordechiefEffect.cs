namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Brutal Hordechief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brutal Hordechief</exampledoc>
[Dependent]
public class BrutalHordechiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures your opponents control block this turn if able, and you choose how those creatures block"];
}
