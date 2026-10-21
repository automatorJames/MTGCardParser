namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An extra combat: "after this phase, there is an additional combat phase", "after this main phase, there is an additional combat phase followed by an additional main phase".</summary>
/// <exampledoc>Fury of the Horde</exampledoc>
/// <examplecapture>after this main phase, there is an additional combat phase followed by an additional main phase</examplecapture>
public class AdditionalPhase : Glyph
{
    public override Nib[] Nibs => ["after this", Alt("phase", "main phase"), ", there is an additional", Prop(Phase), Prop(FollowedByMainPhase)];

    public Phase Phase { get; set; }
    [RegexPattern("followed by an additional main phase")]
    public bool FollowedByMainPhase { get; set; }
}
