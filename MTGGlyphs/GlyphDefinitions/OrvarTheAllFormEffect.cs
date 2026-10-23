namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Orvar, the All-Form, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Orvar, the All-Form</exampledoc>
/// <examplecapture>if it targets one or more other permanents you control, create a token that's a copy of one of those permanents</examplecapture>
[Dependent]
public class OrvarTheAllFormEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it targets one or more other permanents you control, create a token that's a copy of one of those permanents"];
}
