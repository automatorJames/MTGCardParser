namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Yorvo, Lord of Garenbrig, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yorvo, Lord of Garenbrig</exampledoc>
[TokenizationOrder(-1)]
public class YorvoLordOfGarenbrigRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if that creature's power is greater than yorvo's power, put another +1/+1 counter on yorvo"];
}
