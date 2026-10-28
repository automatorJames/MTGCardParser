namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaya the Inexorable, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya the Inexorable</exampledoc>
/// <examplecapture>it gains "when this creature dies or is put into exile, return it to its owner's hand and create a 1/1 white spirit creature token with flying</examplecapture>
[TokenizationOrder(-1)]
public class KayaTheInexorableRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gains \"when this creature dies or is put into exile, return it to its owner's hand and create a 1/1 white spirit creature token with flying"];
}
