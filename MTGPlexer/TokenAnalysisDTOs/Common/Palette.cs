namespace MTGPlexer.TokenAnalysisDTOs.Common;

public record Palette
(
    string Hex,
    string HexLight,
    string HexDark,
    string HexSat,
    string Seed = null
);

