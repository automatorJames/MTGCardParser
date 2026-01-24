namespace CardAnalysisInterface;

public record ContextAction
{
    public ContextActionType Type { get; }
    public string Label { get; }
    public string Color { get; }
    public MaterialIcon Icon { get; }
    public MarkupString MarkupString { get; }
    public bool SectionBreak { get; }

    public ContextAction(ContextActionType type, MaterialIcon? icon = null, string label = null, string color = null, bool sectionBreak = false)
    {
        Type = type;
        Label = label ?? type.ToString().ToFriendlyCase(Extensions.TitleDisplayOption.Lower);
        Color = color ?? GetDefaultColor(type);
        Icon = icon ?? GetDefaultIcon(type);
        MarkupString = GetMarkupString();
        SectionBreak = sectionBreak;
    }

    static MaterialIcon GetDefaultIcon(ContextActionType type) =>
        type switch
        {
            ContextActionType.Delete => MaterialIcon.Delete,
            ContextActionType.ConvertToOneOf => MaterialIcon.Code,
            ContextActionType.ConvertToManyOf => MaterialIcon.Code,
            ContextActionType.ConvertToCompoundOf => MaterialIcon.Code,
            _ => MaterialIcon.None
        };

    static string GetDefaultColor(ContextActionType type) =>
        type switch
        {
            ContextActionType.Delete => "#B07E6C", // brighter, warmer clay
            ContextActionType.ConvertToOneOf => "#9AA864", // livelier olive
            ContextActionType.ConvertToManyOf => "#6EAD8F", // fresher teal-green
            ContextActionType.ConvertToCompoundOf => "#6B97B1", // clearer steel-blue
            _ => "#AAAAAA"
        };

    MarkupString GetMarkupString()
    {
        if (Icon == MaterialIcon.None) return new MarkupString($"<span>{Label}</span>");

        // Get the SVG Path via your extension method
        string pathData = Icon.GetDescription();

        // Build SVG - Note: Material Symbols use 24x24 viewBox
        var svgHtml = $@"
            <svg width=""18"" height=""18"" viewBox=""0 0 24 24"" style=""flex-shrink:0; margin-right:10px;"">
                <path d=""{pathData}"" fill=""{Color}"" />
            </svg>";

        var containerStyle = "display: flex; align-items: center; width: 100%;";
        var textStyle = $"color: {Color}; font-size: 14px; white-space: nowrap;";

        var finalMarkup = $@"
            <div style=""{containerStyle}"">
                {svgHtml}
                <span style=""{textStyle}"">{Label}</span>
            </div>";

        return new MarkupString(finalMarkup);
    }
}