namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ertha Jo, Frontier Mentor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertha Jo, Frontier Mentor</exampledoc>
[TokenizationOrder(-1)]
public class ErthaJoFrontierMentorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you activate an ability that targets a creature or player, copy that ability"];
}
