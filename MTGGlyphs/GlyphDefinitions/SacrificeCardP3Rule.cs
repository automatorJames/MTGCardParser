namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class SacrificeCardP3Rule : Glyph, ICardSpecific
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["add an amount of {b} equal to the ", Nib.This, "d creature's mana value"];
}
