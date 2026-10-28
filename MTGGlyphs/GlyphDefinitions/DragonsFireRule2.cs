namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dragon's Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dragon's Fire</exampledoc>
/// <examplecapture>if you revealed a dragon card or chose a dragon as you cast this spell, {this} deals damage equal to the power of that card or creature instead</examplecapture>
[TokenizationOrder(-1)]
public class DragonsFireRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you revealed a dragon card or chose a dragon as you cast this spell,", Nib.This, "deals damage equal to the power of that card or creature instead"];
}
