namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder that copying a permanent spell makes a token: "a copy of a permanent spell becomes a token", after copy effects.</summary>
/// <exampledoc>See Double</exampledoc>
/// <examplecapture>a copy of a permanent spell becomes a token</examplecapture>
public class CopyOfPermanentSpellRule : Glyph
{
    public override Nib[] Nibs => ["a copy of a permanent spell becomes a token"];
}
