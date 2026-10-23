namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Alluring Siren, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alluring Siren</exampledoc>
[Dependent]
public class AlluringSirenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature an opponent controls attacks you this turn if able"];
}
