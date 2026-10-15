namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Bloodthirst's reminder text.</summary>
/// <exampledoc>Bloodrage Brawler</exampledoc>
/// <examplecapture>if an opponent was dealt damage this turn, this creature enters the battlefield with a +1/+1 counter on it</examplecapture>
public class BloodthirstRule : Glyph
{
    public override Nib[] Nibs => ["if an opponent was dealt damage this turn, this creature enters the battlefield with a +1/+1 counter on it"];
}
