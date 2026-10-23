namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dwarven Sea Clan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dwarven Sea Clan</exampledoc>
[Dependent]
public class DwarvenSeaClanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target attacking or blocking creature whose controller controls an island"];
}
