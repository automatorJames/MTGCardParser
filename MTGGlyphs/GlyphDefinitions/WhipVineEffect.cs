namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Whip Vine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whip Vine</exampledoc>
[Dependent]
public class WhipVineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap target creature with flying blocked by this creature"];
}
