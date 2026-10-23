namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class AnaxHardenedInTheForgeP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the creature had power 4 or greater, create two of those tokens instead"];
}
