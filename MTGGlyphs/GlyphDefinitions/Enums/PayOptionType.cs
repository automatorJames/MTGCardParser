namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How an optional cost is offered: "unless you" ("unless you pay {u}"), "unless they" ("unless they sacrifice a nonland permanent"), or "you may pay" ("you may pay 1 life").</summary>
public enum PayOptionType
{
    UnlessYou,

    UnlessThey,

    [RegexPattern("you may pay")]
    YouMay
}
