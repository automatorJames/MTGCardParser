namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Doc Aurlock, Grizzled Genius, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doc Aurlock, Grizzled Genius</exampledoc>
[TokenizationOrder(-1)]
public class DocAurlockGrizzledGeniusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["plotting cards from your hand costs {2} less"];
}
