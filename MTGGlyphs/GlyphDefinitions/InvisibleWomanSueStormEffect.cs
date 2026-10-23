namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Invisible Woman, Sue Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invisible Woman, Sue Storm</exampledoc>
/// <examplecapture>you may create a 0/4 colorless wall creature token with defender</examplecapture>
[Dependent]
public class InvisibleWomanSueStormEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may create a 0/4 colorless wall creature token with defender"];
}
