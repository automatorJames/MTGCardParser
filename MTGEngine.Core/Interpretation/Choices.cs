namespace MTGEngine.Core;

/// <summary>
/// Named choices: what may be chosen for "choose a color / creature type / card name / basic land type / card type",
/// the kind it's recorded as on the object, and whether an object is "of the chosen type".
/// </summary>
public static class Choices
{
    static readonly ImmutableList<string> _basicLandTypes = ["Plains", "Island", "Swamp", "Mountain", "Forest"];

    static readonly ImmutableList<string> _cardTypes = ["Artifact", "Creature", "Enchantment", "Instant", "Land", "Planeswalker", "Sorcery", "Battle", "Kindred"];

    static readonly ImmutableList<string> _commonCreatureTypes = ["Human", "Elf", "Goblin", "Zombie", "Soldier", "Wizard", "Warrior", "Knight", "Spirit", "Merfolk", "Vampire", "Dragon", "Angel", "Beast", "Elemental", "Cleric", "Rogue", "Shaman", "Faerie", "Sliver"];

    /// <summary>The kind a choice is recorded as: names of any sort as "CardName", land types as "LandType".</summary>
    public static string Category(string kind) =>
        kind switch
        {
            "NonlandCardName" or "CardName" or "NonartifactNonlandCardName" => "CardName",
            "BasicLandType" or "LandType" or "NonbasicLandType" => "LandType",
            _ => kind,
        };

    /// <summary>
    /// The options for a choice of <paramref name="kind"/>, the likeliest first for the chooser: creature types by how
    /// many of the chooser's cards have them, card names opponents' cards first. Null for a kind the engine can't list.
    /// </summary>
    public static ImmutableList<string> Options(string kind, PlayerId chooser, EffectContext ctx, IReadOnlyList<string> except)
    {
        var state = ctx.State;
        IEnumerable<string> options = kind switch
        {
            "Color" => Colors.All.Select(x => x.ToString()),
            "BasicLandType" => _basicLandTypes,
            "LandType" => _basicLandTypes.Concat(state.Objects.Values.Where(x => x.Card.IsLand).SelectMany(x => x.Card.Subtypes).Distinct().Order(StringComparer.Ordinal)),
            "CardType" => _cardTypes,
            "OddOrEven" => ["Odd", "Even"],
            "CreatureType" => state.Objects.Values.Where(x => x.Card.IsCreature || x.Card.Types.Contains("Kindred"))
                .SelectMany(x => x.Card.Subtypes.Select(t => (Type: t, Mine: x.Owner == chooser)))
                .GroupBy(x => x.Type)
                .OrderByDescending(g => g.Count(x => x.Mine)).ThenByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal)
                .Select(g => g.Key)
                .Concat(_commonCreatureTypes),
            "CardName" or "NonlandCardName" or "NonartifactNonlandCardName" => state.Objects.Values
                .Where(x => kind == "CardName" || !x.Card.IsLand && (kind != "NonartifactNonlandCardName" || !x.Card.IsArtifact))
                .Where(x => !x.IsToken)
                .OrderBy(x => x.Owner == chooser ? 1 : 0).ThenBy(x => x.Id.Value)
                .Select(x => x.Card.Name),
            _ => null,
        };
        if (options is null)
            return null;
        return options.Distinct().Where(x => except is null || !except.Contains(x)).ToImmutableList();
    }

    /// <summary>Whether <paramref name="obj"/> is "of the chosen type / color", "with the chosen name", by what <paramref name="holder"/> recorded; null when the characteristic isn't one of those.</summary>
    public static bool? Matches(string characteristic, GameObject holder, GameObject obj, Characteristics traits)
    {
        switch (characteristic)
        {
            case "OfTheChosenType" or "OfThatType" or "OfTheChosenCreatureType":
            {
                var chosen = holder?.Chosen("CreatureType") ?? holder?.Chosen("LandType") ?? holder?.Chosen("CardType");
                return chosen is not null && (traits.Subtypes.Contains(chosen) || traits.Types.Contains(chosen));
            }
            case "OfTheChosenColor" or "OfThatColor":
                return Colors.OfName(holder?.Chosen("Color")) is { } color && traits.Colors.Contains(color);
            case "Name" or "WithTheChosenName" or "OfTheChosenName":
                return holder?.Chosen("CardName") is { } name && obj.Card.Name == name;
            default:
                return null;
        }
    }

    /// <summary>The "of the chosen …" characteristics <see cref="Matches"/> understands.</summary>
    public static readonly ImmutableHashSet<string> Supported = ["OfTheChosenType", "OfThatType", "OfTheChosenCreatureType", "OfTheChosenColor", "OfThatColor", "Name", "WithTheChosenName", "OfTheChosenName"];
}
