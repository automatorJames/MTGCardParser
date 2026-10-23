namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class FirionWildRoseWarriorP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a nontoken equipment you control enters, create a token that's a copy of it, except it has \"this equipment's equip abilities cost {2} less to activate"];
}
