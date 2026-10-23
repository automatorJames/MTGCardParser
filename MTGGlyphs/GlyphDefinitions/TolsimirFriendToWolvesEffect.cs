namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tolsimir, Friend to Wolves, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tolsimir, Friend to Wolves</exampledoc>
[Dependent]
public class TolsimirFriendToWolvesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create voja, friend to elves, a legendary 3/3 green and white wolf creature token"];
}
