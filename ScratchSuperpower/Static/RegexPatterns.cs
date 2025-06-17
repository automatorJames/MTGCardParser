using System.Text.RegularExpressions;

namespace MTGCardParser.Static;

public static class RegexPatterns
{
    public const string Color = @"\b(?<color>white|blue|black|red|green)\b";
    public const string CardType = @"\b(?<cardtype>artifact|creature|enchantment|instant|land|planeswalker|sorcery|battle|tribal)\b";
    public const string LifeChange = @"\b(?<lifeverb>gain|gains|lose|loses)\s+(?<amount>\d+)\s+life\b";
    public const string CardQuantityChange = @"\b(?<verb>discard|discards|draw|draws)\s+(?<amount>a|\d+|one|two|three|four|five|six|seven|eight|nine|ten)\s+card(s)?\b";
    public const string PlusMinusPowerToughness = @"\+(?<power>[0-9]+)/\+(?<toughness>[0-9]+)";
    public const string ManaCost = @"(?<manacost>(?:\{(?:[0-9]+|[wubrgxyzc∞]|w/u|w/b|u/b|u/r|b/r|b/g|r/g|r/w|g/w|g/u|2/w|2/u|2/b|2/r|2/g|p|s)\})+)";

    public static string Keyword = @"\b(?<keyword>" + GetAlternation(Lists.Keywords) + @")\b";
    public static string Subtype = @"\b(?<subtype>" + GetAlternation(Lists.Subtypes) + @")\b";

    public static string GetAlternation(IEnumerable<string> items) => string.Join("|", items.OrderByDescending(k => k.Length).Select(Regex.Escape));
}

