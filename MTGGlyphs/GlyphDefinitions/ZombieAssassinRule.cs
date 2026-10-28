namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zombie Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zombie Assassin</exampledoc>
[TokenizationOrder(-1)]
public class ZombieAssassinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{t}, exile two cards from your graveyard and this creature: destroy target nonblack creature"];
}
