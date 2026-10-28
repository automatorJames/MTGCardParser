namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Adventurers' Guildhouse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Adventurers' Guildhouse</exampledoc>
[TokenizationOrder(-1)]
public class AdventurersGuildhouseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["green legendary creatures you control have \"bands with other legendary creatures"];
}
