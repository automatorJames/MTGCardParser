namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dalkovan Encampment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dalkovan Encampment</exampledoc>
[Dependent]
public class DalkovanEncampmentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you attack this turn, create two 1/1 red warrior creature tokens that are tapped and attacking"];
}
