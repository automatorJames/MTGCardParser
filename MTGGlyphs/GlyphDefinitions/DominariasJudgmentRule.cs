namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dominaria's Judgment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dominaria's Judgment</exampledoc>
/// <examplecapture>until end of turn, creatures you control gain protection from white if you control a plains, from blue if you control an island, from black if you control a swamp, from red if you control a mountain, and from green if you control a forest</examplecapture>
[TokenizationOrder(-1)]
public class DominariasJudgmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, creatures you control gain protection from white if you control a plains, from blue if you control an island, from black if you control a swamp, from red if you control a mountain, and from green if you control a forest"];
}
