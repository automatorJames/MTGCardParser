namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class SwashbucklersWhipP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature has reach, \"{2}, {t}: tap target artifact or creature,\" and \"{8}, {t}: discover 10"];
}
