namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shining Shoal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shining Shoal</exampledoc>
/// <examplecapture>the next x damage that a source of your choice would deal to you and/or creatures you control this turn is dealt to any target instead</examplecapture>
[TokenizationOrder(-1)]
public class ShiningShoalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next x damage that a source of your choice would deal to you and/or creatures you control this turn is dealt to any target instead"];
}
