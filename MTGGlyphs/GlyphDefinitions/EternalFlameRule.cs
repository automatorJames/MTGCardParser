namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eternal Flame, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eternal Flame</exampledoc>
/// <examplecapture>{this} deals x damage to target opponent or planeswalker and half x damage, rounded up, to you, where x is the number of mountains you control</examplecapture>
[TokenizationOrder(-1)]
public class EternalFlameRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals x damage to target opponent or planeswalker and half x damage, rounded up, to you, where x is the number of mountains you control"];
}
