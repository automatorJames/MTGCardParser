namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Giant Shark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Giant Shark</exampledoc>
/// <examplecapture>this creature gets +2/+0 and gains trample until end of turn</examplecapture>
[Dependent]
public class GiantSharkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +2/+0 and gains trample until end of turn"];
}
