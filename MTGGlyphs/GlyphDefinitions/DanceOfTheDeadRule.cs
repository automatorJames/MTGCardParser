namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dance of the Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dance of the Dead</exampledoc>
[TokenizationOrder(-1)]
public class DanceOfTheDeadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put enchanted creature card onto the battlefield tapped under your control and attach this aura to it"];
}
