namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class GlitteringLionP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{3}: until end of turn, this creature loses \"prevent all damage that would be dealt to this creature"];
}
