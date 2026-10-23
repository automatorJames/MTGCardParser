namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tablet of the Guilds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tablet of the Guilds</exampledoc>
/// <examplecapture>if it's at least one of the chosen colors, you gain 1 life for each of the chosen colors it is</examplecapture>
[Dependent]
public class TabletOfTheGuildsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's at least one of the chosen colors, you gain 1 life for each of the chosen colors it is"];
}
