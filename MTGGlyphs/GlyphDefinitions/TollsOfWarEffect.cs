namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tolls of War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tolls of War</exampledoc>
[Dependent]
public class TollsOfWarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 white ally creature token"];
}
