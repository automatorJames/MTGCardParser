namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Niv-Mizzet, Guildpact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niv-Mizzet, Guildpact</exampledoc>
/// <examplecapture>it deals x damage to any target, target player draws x cards, and you gain x life, where x is the number of different color pairs among permanents you control that are exactly two colors</examplecapture>
[Dependent]
public class NivMizzetGuildpactEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals x damage to any target, target player draws x cards, and you gain x life, where x is the number of different color pairs among permanents you control that are exactly two colors"];
}
