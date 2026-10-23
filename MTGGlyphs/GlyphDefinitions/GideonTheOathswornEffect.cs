namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gideon, the Oathsworn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon, the Oathsworn</exampledoc>
[Dependent]
public class GideonTheOathswornEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["he can't attack if he was cast this turn"];
}
