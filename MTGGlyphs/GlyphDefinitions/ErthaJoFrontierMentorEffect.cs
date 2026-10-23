namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ertha Jo, Frontier Mentor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertha Jo, Frontier Mentor</exampledoc>
[Dependent]
public class ErthaJoFrontierMentorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy that ability"];
}
