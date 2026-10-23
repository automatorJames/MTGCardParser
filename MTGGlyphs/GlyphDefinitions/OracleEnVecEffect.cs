namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Oracle en-Vec, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oracle en-Vec</exampledoc>
[Dependent]
public class OracleEnVecEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each of the chosen creatures that didn't attack this turn"];
}
