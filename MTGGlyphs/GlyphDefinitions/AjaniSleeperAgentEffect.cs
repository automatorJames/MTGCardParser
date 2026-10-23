namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ajani, Sleeper Agent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ajani, Sleeper Agent</exampledoc>
[Dependent]
public class AjaniSleeperAgentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["distribute three +1/+1 counters among up to three target creatures"];
}
