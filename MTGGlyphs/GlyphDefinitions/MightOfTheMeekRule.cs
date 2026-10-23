namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Might of the Meek, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Might of the Meek</exampledoc>
[TokenizationOrder(-1)]
public class MightOfTheMeekRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it also gets +1/+0 until end of turn if you control a mouse"];
}
