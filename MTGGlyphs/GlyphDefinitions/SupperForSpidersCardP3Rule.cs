namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class SupperForSpidersCardP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they are food artifacts with \"{2}, {t}, sacrifice this artifact: you gain 3 life"];
}
