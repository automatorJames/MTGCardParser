namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Charmed Griffin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Charmed Griffin</exampledoc>
[Dependent]
public class CharmedGriffinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each other player may put an artifact or enchantment card onto the battlefield from their hand"];
}
