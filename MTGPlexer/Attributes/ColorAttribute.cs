namespace MTGPlexer.Attributes;

public class ColorAttribute(string hexValue) : Attribute
{
    public HexColor Color { get; set; } = new(hexValue);
}

