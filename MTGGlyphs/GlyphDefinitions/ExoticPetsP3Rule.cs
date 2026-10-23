namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class ExoticPetsP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then for each kind of counter among creatures you control, put a counter of that kind on either of those tokens"];
}
