namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Taii Wakeen, Perfect Shot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Taii Wakeen, Perfect Shot</exampledoc>
/// <examplecapture>if a source you control would deal noncombat damage to a permanent or player this turn, it deals that much damage plus x instead</examplecapture>
[Dependent]
public class TaiiWakeenPerfectShotEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a source you control would deal noncombat damage to a permanent or player this turn, it deals that much damage plus x instead"];
}
