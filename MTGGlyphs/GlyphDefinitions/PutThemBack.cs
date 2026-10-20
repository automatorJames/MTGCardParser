namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"them back in any order": returning looked-at cards to the top of the library in an order of the player's choice.</summary>
/// <exampledoc>Aven Fateshaper</exampledoc>
/// <examplecapture>them back in any order</examplecapture>
[Dependent]
public class PutThemBack : Glyph
{
    public override Nib[] Nibs => ["them back in any order"];
}
