namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dion, Bahamut's Dominant // Bahamut, Warden of Light, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dion, Bahamut's Dominant // Bahamut, Warden of Light</exampledoc>
[TokenizationOrder(-1)]
public class DionBahamutsDominantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["dragonfire dive — during your turn, dion and other knights you control have flying"];
}
