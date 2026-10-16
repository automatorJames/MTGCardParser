namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards named earlier in the line, whatever their type: "that card", "those cards", "the exiled card", "the exiled cards".</summary>
/// <exampledoc>Faceless Butcher</exampledoc>
/// <examplecapture>the exiled card</examplecapture>
[Dependent]
public class ThatCardPhrase : Glyph
{
    public override Nib[] Nibs => [Alt("that", "those", "the exiled"), Plural("card")];
}
