namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fearless Swashbuckler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fearless Swashbuckler</exampledoc>
[Dependent]
public class FearlessSwashbucklerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a pirate and a vehicle attacked this combat, draw three cards, then discard two cards"];
}
