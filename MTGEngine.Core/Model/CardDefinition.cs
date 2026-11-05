namespace MTGEngine.Core;

/// <summary>
/// What a card is, independent of any game: its name, cost, type line, printed power/toughness/loyalty, and its
/// abilities as rules IR. Tokens and copies get definitions of their own. Built from <see cref="CardRules"/>
/// (the lowering's output) by <see cref="From"/>.
/// </summary>
public sealed record CardDefinition(
    string Name,
    ManaCost ManaCost,
    ImmutableHashSet<string> Supertypes,
    ImmutableHashSet<string> Types,
    ImmutableHashSet<string> Subtypes,
    int? Power,
    int? Toughness,
    int? Loyalty,
    ImmutableList<Ability> Abilities,
    ImmutableHashSet<string> Keywords,
    ImmutableHashSet<Color> Colors,
    bool IsToken = false,
    string PrintedPower = null,
    string PrintedToughness = null)
{
    static readonly char[] _separators = [',', ' '];

    public static CardDefinition From(CardRules rules)
    {
        var info = rules.Info ?? new CardInfo();
        var cost = ManaCost.Parse(info.ManaCost);
        var abilities = Modal.Rewrite(rules.Lines.SelectMany(x => x.Abilities).Select(Unwrap)).ToImmutableList();
        var keywords = Split(info.Keywords is null ? null : string.Join(",", info.Keywords))
            .Union(abilities.OfType<KeywordLine>().SelectMany(x => x.Keywords).Select(x => Normalize(x.Keyword)))
            .ToImmutableHashSet();

        return new CardDefinition(
            rules.Name,
            cost,
            Split(info.Supertypes),
            Split(rules.Types),
            Split(info.Subtypes),
            Stat(info.Power),
            Stat(info.Toughness),
            Stat(info.Loyalty),
            abilities,
            keywords,
            cost.Colored.Keys.Concat(cost.Hybrid.SelectMany(x => x)).ToImmutableHashSet(),
            PrintedPower: info.Power,
            PrintedToughness: info.Toughness);
    }

    /// <summary>A token's definition from the lowering's description of it.</summary>
    public static CardDefinition FromToken(TokenSpec spec)
    {
        var types = (spec.Types ?? []).Select(Normalize).ToImmutableHashSet();
        var subtypes = (spec.Subtypes ?? []).ToImmutableHashSet();
        var colors = (spec.Colors ?? []).Select(MTGEngine.Core.Colors.OfName).Where(x => x is not null).Select(x => x.Value).ToImmutableHashSet();
        var name = spec.Name ?? (spec.Predefined is not null ? spec.Predefined : string.Join(" ", subtypes.Count > 0 ? subtypes : types));
        var abilities = (spec.Abilities ?? []).ToImmutableList();
        if (spec.Keywords is { Count: > 0 })
            abilities = abilities.Add(new KeywordLine(spec.Keywords));
        if (spec.Predefined is { } predefined && PredefinedAbilities(predefined) is { } given && abilities.Count == 0)
            abilities = given;

        return new CardDefinition(
            name,
            ManaCost.Free,
            ImmutableHashSet<string>.Empty,
            types.Count == 0 && spec.Predefined is not null ? PredefinedTypes(spec.Predefined) : types,
            subtypes.Count == 0 && spec.Predefined is not null ? PredefinedSubtypes(spec.Predefined) : subtypes,
            Stat(spec.Power),
            Stat(spec.Toughness),
            null,
            abilities,
            (spec.Keywords ?? []).Select(x => Normalize(x.Keyword)).ToImmutableHashSet(),
            colors,
            IsToken: true);
    }

    /// <summary>An ability word or flavor word only labels the ability it introduces; the engine acts on the ability.</summary>
    public static Ability Unwrap(Ability ability) =>
        ability switch
        {
            LabeledAbility labeled => Unwrap(labeled.Ability),
            FlavorAbility flavor => Unwrap(flavor.Ability),
            _ => ability,
        };

    static ImmutableHashSet<string> PredefinedTypes(string predefined) =>
        predefined switch
        {
            "Treasure" or "Food" or "Clue" or "Blood" or "Powerstone" or "Gold" or "Map" or "Junk" => ["Artifact"],
            "Role" or "Shard" => ["Enchantment"],
            _ => ["Creature"],
        };

    static ImmutableHashSet<string> PredefinedSubtypes(string predefined) => [predefined];

    /// <summary>The rules text of the predefined tokens, as IR: treasure, food, clue.</summary>
    static ImmutableList<Ability> PredefinedAbilities(string predefined) =>
        predefined switch
        {
            "Treasure" => [new ActivatedAbility([new TapSelf(), new ActionCost(new Sacrifice(new PlayerSelector(PlayerKind.You), new Self()))], new AddMana([new ManaChoice(new Fixed(1), "AnyColor")]))],
            "Food" => [new ActivatedAbility([new PayMana(["{2}"]), new TapSelf(), new ActionCost(new Sacrifice(new PlayerSelector(PlayerKind.You), new Self()))], new GainLife(new PlayerSelector(PlayerKind.You), new Fixed(3)))],
            "Clue" => [new ActivatedAbility([new PayMana(["{2}"]), new ActionCost(new Sacrifice(new PlayerSelector(PlayerKind.You), new Self()))], new Draw(new PlayerSelector(PlayerKind.You), new Fixed(1)))],
            _ => null,
        };

    public static ImmutableHashSet<string> Split(string text) =>
        string.IsNullOrWhiteSpace(text)
            ? ImmutableHashSet<string>.Empty
            : text.Split(_separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Normalize).ToImmutableHashSet();

    /// <summary>A type or keyword word as the vocabulary names it: "first strike" and "FirstStrike" both become "FirstStrike".</summary>
    public static string Normalize(string word) =>
        string.IsNullOrEmpty(word) ? word : string.Concat(word.Split([' ', '-', '\''], StringSplitOptions.RemoveEmptyEntries).Select(x => char.ToUpperInvariant(x[0]) + x[1..]));

    static int? Stat(string text) => int.TryParse(text, out var value) ? value : null;

    static int? Stat(Quantity quantity) => quantity is Fixed fixedValue ? fixedValue.Value : null;

    public bool IsCreature => Types.Contains("Creature");
    public bool IsLand => Types.Contains("Land");
    public bool IsInstant => Types.Contains("Instant");
    public bool IsSorcery => Types.Contains("Sorcery");
    public bool IsArtifact => Types.Contains("Artifact");
    public bool IsEnchantment => Types.Contains("Enchantment");
    public bool IsPlaneswalker => Types.Contains("Planeswalker");
    public bool IsAura => Subtypes.Contains("Aura");
    public bool IsEquipment => Subtypes.Contains("Equipment");
    public bool IsPermanentCard => IsCreature || IsLand || IsArtifact || IsEnchantment || IsPlaneswalker || Types.Contains("Battle");
    public bool IsBasicLand => IsLand && Supertypes.Contains("Basic");

    /// <summary>The spell's effect(s) when it's an instant or sorcery: every spell ability's effect, in order.</summary>
    public IEnumerable<Effect> SpellEffects => Abilities.OfType<SpellAbility>().Select(x => x.Effect);

    public override string ToString() => Name;
}
