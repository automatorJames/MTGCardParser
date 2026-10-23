namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Aetherspark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Aetherspark</exampledoc>
/// <examplecapture>as long as {this} is attached to a creature, {this} can't be attacked and has "whenever equipped creature deals combat damage during your turn, put that many loyalty counters on {this}</examplecapture>
[TokenizationOrder(-1)]
public class TheAethersparkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as", Nib.This, "is attached to a creature,", Nib.This, "can't be attacked and has \"whenever equipped creature deals combat damage during your turn, put that many loyalty counters on", Nib.This];
}
