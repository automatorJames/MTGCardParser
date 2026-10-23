namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glaive of the Guildpact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glaive of the Guildpact</exampledoc>
[TokenizationOrder(-1)]
public class GlaiveOfTheGuildpactRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +1/+0 for each gate you control and has vigilance and menace"];
}
