namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Adeline, Resplendent Cathar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Adeline, Resplendent Cathar</exampledoc>
[Dependent]
public class AdelineResplendentCatharEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each opponent, create a 1/1 white human creature token that's tapped and attacking that player or a planeswalker they control"];
}
