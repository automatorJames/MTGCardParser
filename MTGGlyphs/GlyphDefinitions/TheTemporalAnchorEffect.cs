namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Temporal Anchor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Temporal Anchor</exampledoc>
/// <examplecapture>exile that many cards from the bottom of your library</examplecapture>
[Dependent]
public class TheTemporalAnchorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that many cards from the bottom of your library"];
}
