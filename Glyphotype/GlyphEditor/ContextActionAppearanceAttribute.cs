namespace Glyphotype.GlyphEditor;

[AttributeUsage(AttributeTargets.Field)]
public class ContextActionAppearanceAttribute : Attribute
{
    public string Label { get; }
    public MaterialIcon Icon { get; }
    public ContextMenuColor Color { get; }
    public bool AddSectionBreak { get; }

    public ContextActionAppearanceAttribute(string label, MaterialIcon icon, ContextMenuColor color, bool addSectionBreak = false)
    {
        Label = label;
        Icon = icon;
        Color = color;
        AddSectionBreak = addSectionBreak;
    }

    public string GetHtml()
    {
        string pathData = Icon.GetDescription();
        var colorHex = Color.GetDescription();

        var containerStyle = $"display: flex; align-items: center; width: 100%; color: {colorHex}; font-size: 14px; white-space: nowrap;";
        //ar textStyle = $"color: {colorHex}; font-size: 14px; white-space: nowrap;";

        var finalMarkup = $@"
            <div style=""{containerStyle}"">
                <span class=""material-symbols-outlined"">{Icon}</span>
                &nbsp;
                <span>{Label}</span>
            </div>";

        return finalMarkup;
    }
}
