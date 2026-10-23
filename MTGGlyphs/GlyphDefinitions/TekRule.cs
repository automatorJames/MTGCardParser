namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tek, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tek</exampledoc>
/// <examplecapture>this creature gets +0/+2 as long as you control a plains, has flying as long as you control an island, gets +2/+0 as long as you control a swamp, has first strike as long as you control a mountain, and has trample as long as you control a forest</examplecapture>
[TokenizationOrder(-1)]
public class TekRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +0/+2 as long as you control a plains, has flying as long as you control an island, gets +2/+0 as long as you control a swamp, has first strike as long as you control a mountain, and has trample as long as you control a forest"];
}
