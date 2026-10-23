namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Arcum's Whistle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arcum's Whistle</exampledoc>
/// <examplecapture>if they don't pay, the creature attacks this turn if able, and at the beginning of the next end step, destroy it if it didn't attack this turn</examplecapture>
[TokenizationOrder(-1)]
public class ArcumsWhistleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they don't pay, the creature attacks this turn if able, and at the beginning of the next end step, destroy it if it didn't attack this turn"];
}
