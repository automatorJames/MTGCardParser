namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spectrum Sentinel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spectrum Sentinel</exampledoc>
[TokenizationOrder(-1)]
public class SpectrumSentinelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["protection from multicolored (this creature can't be blocked, targeted, dealt damage, enchanted, or equipped by anything multicolored"];
}
