namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chrome Replicator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chrome Replicator</exampledoc>
/// <examplecapture>if you control two or more nonland, nontoken permanents with the same name as one another, create a 4/4 colorless construct artifact creature token</examplecapture>
[Dependent]
public class ChromeReplicatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control two or more nonland, nontoken permanents with the same name as one another, create a 4/4 colorless construct artifact creature token"];
}
