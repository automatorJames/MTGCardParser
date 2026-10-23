namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Doomsday Excruciator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doomsday Excruciator</exampledoc>
[Dependent]
public class DoomsdayExcruciatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it was cast, each player exiles all but the bottom six cards of their library face down"];
}
