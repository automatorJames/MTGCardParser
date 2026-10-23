namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Whiplash, Vengeful Engineer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whiplash, Vengeful Engineer</exampledoc>
/// <examplecapture>if he's equipped, each opponent loses x life and you gain x life, where x is the number of equipment attached to him</examplecapture>
[Dependent]
public class WhiplashVengefulEngineerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if he's equipped, each opponent loses x life and you gain x life, where x is the number of equipment attached to him"];
}
