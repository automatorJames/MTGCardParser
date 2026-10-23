namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ashiok, Wicked Manipulator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashiok, Wicked Manipulator</exampledoc>
[Dependent]
public class AshiokWickedManipulator3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player exiles the top x cards of their library, where x is the total mana value of cards you own in exile"];
}
