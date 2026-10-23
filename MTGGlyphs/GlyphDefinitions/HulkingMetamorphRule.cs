namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hulking Metamorph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hulking Metamorph</exampledoc>
/// <examplecapture>you may have this creature enter as a copy of an artifact or creature you control, except it's an artifact creature in addition to its other types, and its power and toughness are equal to this creature's power and toughness</examplecapture>
[TokenizationOrder(-1)]
public class HulkingMetamorphRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature enter as a copy of an artifact or creature you control, except it's an artifact creature in addition to its other types, and its power and toughness are equal to this creature's power and toughness"];
}
