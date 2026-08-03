namespace MTGPlexer.Colors;

public record HexPalette
(
    string Normal,
    string Light,
    string Dark,
    string Sat,
    PaletteVariant? Variant = null
)
{
    public string GetVariant(PaletteVariant variant) =>
        variant switch
        {
            PaletteVariant.Normal => Normal,
            PaletteVariant.Light => Light,
            PaletteVariant.Dark => Dark,
            PaletteVariant.Sat => Sat,
            _ => throw new NotImplementedException($"{variant} not supported")
        };

    public string ToColorStyle(PaletteVariant? overrideVariant = null)
    {
        var variant = overrideVariant ?? Variant ?? PaletteVariant.Normal;

        string style = variant switch
        {
            PaletteVariant.Dark =>
                $"--color: {Dark}; --highlight-color: {Normal}; --lowlight-color: {Dark};",

            PaletteVariant.Light =>
                $"--color: {Light}; --highlight-color: {Light}; --lowlight-color: {Normal};",

            PaletteVariant.Sat =>
                $"--color: {Sat}; --highlight-color: {Light}; --lowlight-color: {Dark};",

            PaletteVariant.Normal or _ =>
                $"--color: {Normal}; --highlight-color: {Light}; --lowlight-color: {Dark};",
        };

        return style;
    }
}

public enum PaletteVariant
{
    Normal,
    Light,
    Dark,
    Sat
}

