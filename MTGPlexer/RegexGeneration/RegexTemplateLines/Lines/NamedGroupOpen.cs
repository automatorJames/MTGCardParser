namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record NamedGroupOpen
(
    Enclosure[] Enclosures,
    string Name,
    RegexPropInfo Prop,
    string CaptureType, 
    string NameOverride = null
)
    : EncloureBookend
    (
        Enclosures: Enclosures,
        Regex: RenderCaptureGroup(Prop, NameOverride),
        Comment: CaptureType
    )
{
    static string RenderCaptureGroup(RegexPropInfo prop, string nameOverride)
        => $"(?<{prop?.Name ?? nameOverride ?? ""}>";

    public override string ToString() => base.ToString();
}