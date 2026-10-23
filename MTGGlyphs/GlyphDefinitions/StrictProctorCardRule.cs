namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Strict Proctor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Strict Proctor</exampledoc>
[TokenizationOrder(-1)]
public class StrictProctorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a permanent entering causes a triggered ability to trigger, counter that ability unless its controller pays {2}"];
}
