namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ghostly Dancers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ghostly Dancers</exampledoc>
[Dependent]
public class GhostlyDancersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return an enchantment card from your graveyard to your hand or unlock a locked door of a room you control"];
}
