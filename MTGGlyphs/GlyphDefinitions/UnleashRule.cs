namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Unleash's reminder text, first sentence.</summary>
/// <exampledoc>Bloodfray Giant</exampledoc>
/// <examplecapture>you may have this creature enter with a +1/+1 counter on it</examplecapture>
public class UnleashRule : Glyph
{
    public override Nib[] Nibs => ["you may have this creature", Alt("enter", "enter the battlefield"), "with a +1/+1 counter on it"];
}
