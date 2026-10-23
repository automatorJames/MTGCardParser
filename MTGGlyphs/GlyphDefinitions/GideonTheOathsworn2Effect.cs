namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gideon, the Oathsworn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon, the Oathsworn</exampledoc>
[Dependent]
public class GideonTheOathsworn2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile gideon and each creature your opponents control"];
}
