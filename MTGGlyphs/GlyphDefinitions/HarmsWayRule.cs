namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Harm's Way, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Harm's Way</exampledoc>
/// <examplecapture>the next 2 damage that a source of your choice would deal to you and/or permanents you control this turn is dealt to any target instead</examplecapture>
[TokenizationOrder(-1)]
public class HarmsWayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next 2 damage that a source of your choice would deal to you and/or permanents you control this turn is dealt to any target instead"];
}
