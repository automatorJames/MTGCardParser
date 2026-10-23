namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Elkin Lair, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elkin Lair</exampledoc>
[Dependent]
public class ElkinLairEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the player hasn't played the card, they put it into their graveyard"];
}
