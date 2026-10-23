namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hama Pashar, Ruin Seeker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hama Pashar, Ruin Seeker</exampledoc>
[TokenizationOrder(-1)]
public class HamaPasharRuinSeekerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["room abilities of dungeons you own trigger an additional time"];
}
