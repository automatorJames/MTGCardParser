namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bronzehide Lion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bronzehide Lion</exampledoc>
/// <examplecapture>it's an aura enchantment with enchant creature you control and "{g}{w}: enchanted creature gains indestructible until end of turn," and it loses all other abilities</examplecapture>
[TokenizationOrder(-1)]
public class BronzehideLionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's an aura enchantment with enchant creature you control and \"{g}{w}: enchanted creature gains indestructible until end of turn,\" and it loses all other abilities"];
}
