namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Agent Maria Hill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Agent Maria Hill</exampledoc>
[Dependent]
public class AgentMariaHillEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on her and draw a card"];
}
