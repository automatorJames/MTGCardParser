namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Unlocking a door of a room: "you unlock this door".</summary>
/// <exampledoc>Ticket Booth // Tunnel of Hate</exampledoc>
/// <examplecapture>you unlock this door</examplecapture>
[Dependent]
[RegexPattern("you unlock this door")]
public class UnlockThisDoor : Glyph;
