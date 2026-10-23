namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thief's Knife, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thief's Knife</exampledoc>
/// <examplecapture>equipped creature gets +1/+1, has "whenever this creature deals combat damage to a player, draw a card," and is a rogue in addition to its other types</examplecapture>
[TokenizationOrder(-1)]
public class ThiefsKnifeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +1/+1, has \"whenever this creature deals combat damage to a player, draw a card,\" and is a rogue in addition to its other types"];
}
