namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Mycotyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Mycotyrant</exampledoc>
/// <examplecapture>create x 1/1 black fungus creature tokens with "this token can't block," where x is the number of times you descended this turn</examplecapture>
[Dependent]
public class TheMycotyrantEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create x 1/1 black fungus creature tokens with \"this token can't block,\" where x is the number of times you descended this turn"];
}
