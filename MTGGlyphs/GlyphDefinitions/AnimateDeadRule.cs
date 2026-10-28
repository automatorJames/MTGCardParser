namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Animate Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Animate Dead</exampledoc>
[TokenizationOrder(-1)]
public class AnimateDeadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this aura enters, if it's on the battlefield, it loses \"enchant creature card in a graveyard\" and gains \"enchant creature put onto the battlefield with this aura"];
}
