namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gonti, Night Minister, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gonti, Night Minister</exampledoc>
/// <examplecapture>its controller looks at the top card of that opponent's library and exiles it face down</examplecapture>
[Dependent]
public class GontiNightMinisterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller looks at the top card of that opponent's library and exiles it face down"];
}
