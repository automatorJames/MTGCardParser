namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class FlyP3Rule : Glyph, ICardSpecific
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["enchanted creature has ", Nib.This, "ing and \"whenever this creature deals combat damage to a player, venture into the dungeon"];
}
