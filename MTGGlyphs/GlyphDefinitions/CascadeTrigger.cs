namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cascade's reminder text, first sentence.</summary>
/// <exampledoc>Ardent Plea</exampledoc>
/// <examplecapture>when you cast this spell, exile cards from the top of your library until you exile a nonland card that costs less</examplecapture>
public class CascadeTrigger : Glyph
{
    public override Nib[] Nibs => ["when you cast this spell, exile cards from the top of your library until you exile a nonland card that costs less"];
}
