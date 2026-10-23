namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class HallowP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage target spell would deal this turn"];
}
