namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Defense of the Heart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Defense of the Heart</exampledoc>
/// <examplecapture>if an opponent controls three or more creatures, sacrifice this enchantment, search your library for up to two creature cards, put those cards onto the battlefield, then shuffle</examplecapture>
[Dependent]
public class DefenseOfTheHeartEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent controls three or more creatures, sacrifice this enchantment, search your library for up to two creature cards, put those cards onto the battlefield, then shuffle"];
}
