namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record NamedGroupOpen
(
    Enclosure[] Enclosures,
    string Name,
    RegexPropInfo Prop,
    string CaptureType, 
    Palette Palette,
    string NameOverride = null
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: RenderCaptureGroup(Prop, NameOverride),
        Palette: Palette, 
        Comment: CaptureType
    )
{
    static string RenderCaptureGroup(RegexPropInfo prop, string nameOverride)
        => $"(?<{prop?.Name ?? nameOverride ?? ""}>";

    public override string ToString() => base.ToString();
}