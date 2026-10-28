namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Platinum Emperion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Platinum Emperion</exampledoc>
[TokenizationOrder(-1)]
public class PlatinumEmperionRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't pay any amount of life except 0"];
}
