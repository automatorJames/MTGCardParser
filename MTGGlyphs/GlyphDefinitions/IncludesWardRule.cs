namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder that "can't be countered" covers ward too: "this includes by the ward ability".</summary>
/// <exampledoc>Heated Debate</exampledoc>
/// <examplecapture>this includes by the ward ability</examplecapture>
public class IncludesWardRule : Glyph
{
    public override Nib[] Nibs => ["this includes by the ward ability"];
}
