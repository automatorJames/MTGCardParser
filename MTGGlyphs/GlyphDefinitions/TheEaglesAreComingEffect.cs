namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Eagles Are Coming!, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Eagles Are Coming!</exampledoc>
/// <examplecapture>create a 4/4 white bird soldier creature token with flying for each creature returned to your hand this way</examplecapture>
[Dependent]
public class TheEaglesAreComingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 4/4 white bird soldier creature token with flying for each creature returned to your hand this way"];
}
