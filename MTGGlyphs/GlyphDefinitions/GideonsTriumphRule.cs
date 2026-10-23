namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gideon's Triumph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon's Triumph</exampledoc>
[TokenizationOrder(-1)]
public class GideonsTriumphRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control a gideon planeswalker, that player sacrifices two of those creatures of their choice instead"];
}
