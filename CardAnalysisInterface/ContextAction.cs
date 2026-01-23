
namespace CardAnalysisInterface;

public record ContextAction
{
    public ContextActionType Type { get; }
    public string Label { get; }
    public string Color { get; }
    public string IconName { get; }
    public MarkupString MarkupString { get; }

    public ContextAction(ContextActionType type, string iconName = null, string label = null, string color = null)
    {
        Type = type;
        Label = label ?? type.ToString().ToFriendlyCase(Extensions.TitleDisplayOption.Lower);
        Color = color ?? GetDefaultColor(type);
        IconName = iconName ?? GetMaterialIconName(type);
        MarkupString = GetMarkupString();
    }

    static string GetMaterialIconName(ContextActionType type) =>
        type switch
        {
            ContextActionType.Delete => "delete",
            ContextActionType.ConvertToOneOf => "code",
            ContextActionType.ConvertToManyOf => "code",
            ContextActionType.ConvertToCompoundOf => "code",
            _ => string.Empty
        };

    static string GetDefaultColor(ContextActionType type) =>
        type switch
        {
            ContextActionType.Delete =>                 "#A37362", // Terracotta
            ContextActionType.ConvertToOneOf =>         "#858F5C", // Olive
            ContextActionType.ConvertToManyOf =>        "#5E947A", // Sage
            ContextActionType.ConvertToCompoundOf =>    "#5E8399", // Slate
            //ContextActionType.Delete => "#7D7394", // Thistle
            //ContextActionType.Delete => "#A36280", // Rose
        };

    MarkupString GetMarkupString()
    {
        var iconStyle = $"style=\"color: {Color}; font-size: 18px;\"";
        var textStyle = $"style=\"color: {Color}; font-size: 14px;\"";
        var str = $"<span class=\"material-symbols-outlined\" {iconStyle}>";
        str += IconName;
        str += $"</span>";
        str += $"<span {textStyle}>{Label}</span>";

        return new(str);
    }
}