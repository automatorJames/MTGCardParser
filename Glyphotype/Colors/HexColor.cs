namespace Glyphotype.Colors;

public record HexColor(string Value)
{
    // Whites
    public static HexColor White => new("#FFFFFF");
    public static HexColor Snow => new("#FFFAFA");
    public static HexColor Ivory => new("#FFFFF0");
    public static HexColor FloralWhite => new("#FFFAF0");
    public static HexColor GhostWhite => new("#F8F8FF");
    public static HexColor WhiteSmoke => new("#F5F5F5");

    // Greys
    public static HexColor Gainsboro => new("#DCDCDC");
    public static HexColor LightGrey => new("#D3D3D3");
    public static HexColor Silver => new("#C0C0C0");
    public static HexColor DarkGrey => new("#A9A9A9");
    public static HexColor Grey => new("#808080");
    public static HexColor DimGrey => new("#696969");
    public static HexColor DarkSlateGrey => new("#2F4F4F");

    // Blacks
    public static HexColor Black => new("#000000");
    public static HexColor NearBlack => new("#111111");
    public static HexColor Charcoal => new("#333333");

    // Primary / basic colors
    public static HexColor Red => new("#FF0000");
    public static HexColor Green => new("#008000");
    public static HexColor Blue => new("#0000FF");

    // Common secondary colors
    public static HexColor Yellow => new("#FFFF00");
    public static HexColor Cyan => new("#00FFFF");
    public static HexColor Magenta => new("#FF00FF");
    public static HexColor Orange => new("#FFA500");
    public static HexColor Purple => new("#800080");
    public static HexColor Brown => new("#A52A2A");
}

