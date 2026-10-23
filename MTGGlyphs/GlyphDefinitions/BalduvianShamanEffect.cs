namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Balduvian Shaman, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balduvian Shaman</exampledoc>
/// <examplecapture>change the text of target white enchantment you control that doesn't have cumulative upkeep by replacing all instances of one color word with another</examplecapture>
[Dependent]
public class BalduvianShamanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["change the text of target white enchantment you control that doesn't have cumulative upkeep by replacing all instances of one color word with another"];
}
