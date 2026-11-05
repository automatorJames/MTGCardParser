namespace MTGEngine.Core;

/// <summary>The keywords whose rules the engine carries out, so the interpret report counts a keyword line executable only when it is.</summary>
public static class Keywords
{
    public static readonly ImmutableHashSet<string> Supported =
    [
        // Combat and evasion, read by the combat rules and damage.
        "Flying", "Reach", "Trample", "Vigilance", "Haste", "FirstStrike", "DoubleStrike", "Deathtouch", "Lifelink", "Menace", "Defender",
        "Indestructible", "Hexproof", "Shroud", "Infect", "Fear", "Intimidate", "Shadow", "Horsemanship",
        // Casting.
        "Flash", "Kicker", "Convoke", "Flashback", "Buyback", "Affinity",
        // Expanded into abilities (see KeywordAbilities).
        "Equip", "Cycling", "Unearth", "Crew", "Prowess", "Exalted", "Flanking", "Bushido", "Bloodthirst", "Ward", "Echo", "CumulativeUpkeep",
        "Scry", "Surveil", "Investigate", "Proliferate", "Fateseal", "Populate", "Bolster", "Explore", "Adapt",
        // Read where they apply.
        "Protection", "Devoid", "Changeling", "Wither", "Toxic", "Persist", "Undying",
        "Swampwalk", "Islandwalk", "Forestwalk", "Mountainwalk", "Plainswalk",
        // Cast face down, turned face up for their cost.
        "Morph", "Megamorph", "Disguise",
    ];

    /// <summary>
    /// Whether <paramref name="protectedObj"/> has protection from <paramref name="source"/>: from a color, all colors,
    /// multicolored, everything, or a card type or creature type ("from artifacts", "from goblins").
    /// </summary>
    public static bool ProtectedFrom(GameState state, GameObject protectedObj, Characteristics traits, GameObject source)
    {
        if (source is null || !traits.HasKeyword("Protection"))
            return false;
        var theirs = Characteristics.Of(state, source, withStatics: false);
        foreach (var keyword in traits.Keywords.Where(x => x.StartsWith("ProtectionFrom", StringComparison.Ordinal)))
        {
            var quality = keyword["ProtectionFrom".Length..];
            var holds = quality switch
            {
                "Everything" => true,
                "AllColors" or "EachColor" => theirs.Colors.Count > 0,
                "Multicolored" => theirs.Colors.Count > 1,
                "Monocolored" => theirs.Colors.Count == 1,
                "Colorless" => theirs.Colors.Count == 0,
                "TheChosenColor" => Colors.OfName(protectedObj.Chosen("Color")) is { } chosen && theirs.Colors.Contains(chosen),
                _ when Colors.OfName(quality) is { } color => theirs.Colors.Contains(color),
                _ => Singular(quality) is var kind && (theirs.Types.Contains(kind) || theirs.Subtypes.Contains(kind)),
            };
            if (holds)
                return true;
        }
        return false;
    }

    static string Singular(string plural) =>
        plural.EndsWith("ies") ? plural[..^3] + "y" : plural.EndsWith("ves") ? plural[..^3] + "f" : plural.EndsWith('s') ? plural[..^1] : plural;

    /// <summary>The land type a landwalk keyword names ("Swampwalk" → "Swamp"); null for none.</summary>
    public static string Landwalk(string keyword) =>
        keyword.EndsWith("walk", StringComparison.Ordinal) && keyword.Length > 4 && keyword != "Landwalk" ? keyword[..^4] : null;

    /// <summary>Whether the engine carries out <paramref name="keyword"/> (in any spelling the IR uses).</summary>
    public static bool Handles(string keyword) => Supported.Contains(CardDefinition.Normalize(keyword));
}
