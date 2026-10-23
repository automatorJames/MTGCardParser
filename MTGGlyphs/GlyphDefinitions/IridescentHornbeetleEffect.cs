namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Iridescent Hornbeetle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Iridescent Hornbeetle</exampledoc>
/// <examplecapture>create a 1/1 green insect creature token for each +1/+1 counter you've put on creatures under your control this turn</examplecapture>
[Dependent]
public class IridescentHornbeetleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 green insect creature token for each +1/+1 counter you've put on creatures under your control this turn"];
}
