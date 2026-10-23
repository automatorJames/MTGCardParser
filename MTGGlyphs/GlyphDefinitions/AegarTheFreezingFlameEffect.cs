namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aegar, the Freezing Flame, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aegar, the Freezing Flame</exampledoc>
/// <examplecapture>if a giant, wizard, or spell you controlled dealt damage to it this turn, draw a card</examplecapture>
[Dependent]
public class AegarTheFreezingFlameEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a giant, wizard, or spell you controlled dealt damage to it this turn, draw a card"];
}
