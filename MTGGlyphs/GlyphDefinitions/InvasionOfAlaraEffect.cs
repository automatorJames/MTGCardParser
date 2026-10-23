namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Invasion of Alara // Awaken the Maelstrom, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion of Alara // Awaken the Maelstrom</exampledoc>
[Dependent]
public class InvasionOfAlaraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile cards from the top of your library until you exile two nonland cards with mana value 4 or less"];
}
