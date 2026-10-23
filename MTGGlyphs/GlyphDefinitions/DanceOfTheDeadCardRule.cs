namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dance of the Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dance of the Dead</exampledoc>
[TokenizationOrder(-1)]
public class DanceOfTheDeadCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this aura enters, if it's on the battlefield, it loses \"enchant creature card in a graveyard\" and gains \"enchant creature put onto the battlefield with this aura put enchanted creature card onto the battlefield tapped under your control and attach this aura to it"];
}
