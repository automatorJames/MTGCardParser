namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ice Cauldron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ice Cauldron</exampledoc>
[Dependent]
public class IceCauldronEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add this artifact's last noted type and amount of mana"];
}
