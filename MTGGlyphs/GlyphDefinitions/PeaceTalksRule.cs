namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Peace Talks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Peace Talks</exampledoc>
[TokenizationOrder(-1)]
public class PeaceTalksRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this turn and next turn, creatures can't attack, and players and permanents can't be the targets of spells or activated abilities"];
}
