namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Menace's reminder text.</summary>
/// <exampledoc>Goblin Heelcutter</exampledoc>
/// <examplecapture>this creature can't be blocked except by two or more creatures</examplecapture>
public class MenaceRule : Glyph
{
    public override Nib[] Nibs => ["this creature can't be blocked except by two or more creatures"];
}
