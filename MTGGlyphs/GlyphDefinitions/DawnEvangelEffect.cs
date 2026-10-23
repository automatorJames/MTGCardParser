namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dawn Evangel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dawn Evangel</exampledoc>
/// <examplecapture>if an aura you controlled was attached to it, return target creature card with mana value 2 or less from your graveyard to your hand</examplecapture>
[Dependent]
public class DawnEvangelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an aura you controlled was attached to it, return target creature card with mana value 2 or less from your graveyard to your hand"];
}
