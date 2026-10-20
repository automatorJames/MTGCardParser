namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Suspend's last reminder sentence: once the last time counter is removed, the card is cast for free ("you may cast" in current wording).</summary>
/// <exampledoc>Rift Bolt</exampledoc>
/// <examplecapture>when the last is removed, you may cast it without paying its mana cost</examplecapture>
public class SuspendLastRemoved : Glyph
{
    public override Nib[] Nibs => ["when the last is removed,", Opt("you may"), "cast it without paying its mana cost"];
}
