namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dance of the Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dance of the Dead</exampledoc>
/// <examplecapture>if it's on the battlefield, it loses "enchant creature card in a graveyard" and gains "enchant creature put onto the battlefield with this aura put enchanted creature card onto the battlefield tapped under your control and attach this aura to it</examplecapture>
[Dependent]
public class DanceOfTheDeadEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's on the battlefield, it loses \"enchant creature card in a graveyard\" and gains \"enchant creature put onto the battlefield with this aura put enchanted creature card onto the battlefield tapped under your control and attach this aura to it"];
}
