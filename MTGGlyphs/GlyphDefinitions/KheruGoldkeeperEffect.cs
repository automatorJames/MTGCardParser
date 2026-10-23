namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kheru Goldkeeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kheru Goldkeeper</exampledoc>
[Dependent]
public class KheruGoldkeeperEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put two +1/+1 counters and a flying counter on target creature"];
}
