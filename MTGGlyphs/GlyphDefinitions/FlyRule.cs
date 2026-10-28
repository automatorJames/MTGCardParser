namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fly, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fly</exampledoc>
[TokenizationOrder(-1)]
public class FlyRule : Glyph, ICardSpecific
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["enchanted creature has ", Nib.This, "ing and \"whenever this creature deals combat damage to a player, venture into the dungeon"];
}
