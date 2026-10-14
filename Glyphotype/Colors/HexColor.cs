namespace Glyphotype.Colors;

public record HexColor(string Value)
{
    public static HexColor White => new("#FFFFFF");
    public static HexColor WhiteSmoke => new("#F5F5F5");
    public static HexColor Silver => new("#C0C0C0");
    public static HexColor DimGrey => new("#696969");
}

