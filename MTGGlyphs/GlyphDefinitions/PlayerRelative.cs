namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What picks out players after "who": controlling a permanent of a kind, having searched this way, or having done (or not) what was just offered.</summary>
/// <exampledoc>Hired Giant</exampledoc>
/// <examplecapture>searched their library this way</examplecapture>
[Dependent]
public class PlayerRelative : GlyphOneOf
{
    public ControlsAPermanent Controls { get; set; }
    public SearchedThisWay Searched { get; set; }
    public PlayerResponse? Response { get; set; }
}
