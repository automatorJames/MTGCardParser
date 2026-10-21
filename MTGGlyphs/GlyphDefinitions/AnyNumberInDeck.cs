namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The deck-construction exemption: "a deck can have any number of cards named {this}".</summary>
/// <exampledoc>Relentless Rats</exampledoc>
/// <examplecapture>a deck can have any number of cards named {this}</examplecapture>
public class AnyNumberInDeck : Glyph
{
    public override Nib[] Nibs => ["a deck can have any number of cards named", Nib.This];
}
