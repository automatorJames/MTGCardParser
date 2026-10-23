namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class VerdantKrakenP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's affected by summoning sickness until your next turn"];
}
