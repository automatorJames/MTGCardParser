namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Swift Warkite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swift Warkite</exampledoc>
[Dependent]
public class SwiftWarkiteEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a creature card with mana value 3 or less from your hand or graveyard onto the battlefield"];
}
