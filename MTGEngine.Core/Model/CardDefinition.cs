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
        var spell = rules.Types?.Contains("Instant") == true || rules.Types?.Contains("Sorcery") == true;
        // On an instant or sorcery, "if that creature would die this turn, exile it instead" is set up as the spell resolves.
        var abilities = InsteadRewrite.Rewrite(Modal.Rewrite(rules.Lines.SelectMany(WithoutCastingReminders).Select(Unwrap)
            .Select(x => spell && x is ReplacementAbility { Effect: Replacement replacement } ? new SpellAbility(replacement) : x)
            .Select(x => spell ? x : AsStatic(x)))).ToImmutableList();
        abilities = Levels.Rewrite(KeywordAbilities.Expand(abilities, spell)).ToImmutableList();
        // A card's keywords are its keyword lines'. The database's keyword list also names ability words, keyword
        // actions, and a double-faced card's back face (Delver of Secrets would fly), so it isn't read.
        var keywords = abilities.OfType<KeywordLine>().SelectMany(x => x.Keywords).SelectMany(KeywordAbilities.NamesOf).ToImmutableHashSet();
        var colors = keywords.Contains("Devoid") ? ImmutableHashSet<Color>.Empty : cost.Colored.Keys.Concat(cost.Hybrid.SelectMany(x => x)).ToImmutableHashSet();

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
            colors,
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
        abilities = KeywordAbilities.Expand(abilities, false).ToImmutableList();
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
            (spec.Keywords ?? []).SelectMany(KeywordAbilities.NamesOf).ToImmutableHashSet(),
            colors,
            IsToken: true);
    }

    /// <summary>Keywords that change how a spell is cast; their reminder text describes that, not something the spell does.</summary>
    static readonly ImmutableHashSet<string> _castingKeywords =
    [
        "Flashback", "JumpStart", "Harmonize", "Overload", "Gift", "Awaken", "Storm", "Conspire", "Convoke", "Retrace", "Escape", "Replicate",
        "Splice", "Buyback", "Entwine", "Kicker", "Multikicker", "Cascade", "Delve", "Emerge", "Spectacle", "Surge", "Madness", "Miracle",
        "Foretell", "Disturb", "Casualty", "Bargain", "Cleave", "Squad", "Plot", "Improvise", "Affinity", "Evoke", "Dash", "Blitz", "Prowl", "Rebound", "Suspend",
    ];

    /// <summary>
    /// A line's abilities without what a casting keyword's reminder text was lowered to: "flashback {1}{u} (… then
    /// exile it.)" is the keyword, not a spell that exiles something. Reminders of keyword actions and triggered
    /// keywords ("fading", "extort") state the keyword's own rules and stay.
    /// </summary>
    public static IEnumerable<Ability> WithoutCastingReminders(LineRules line)
    {
        var abilities = line.Abilities;
        var keywordAt = -1;
        for (var i = 0; i < abilities.Count; i++)
            if (abilities[i] is KeywordLine keywords && keywords.Keywords.Any(k => _castingKeywords.Contains(Normalize(k.Keyword))))
            {
                keywordAt = i;
                break;
            }
        if (line.Text?.Contains('(') != true)
            return abilities;
        // A line of keywords the engine carries out: what follows them is their reminder text, which the engine doesn't need.
        for (var i = 0; i < abilities.Count; i++)
            if (abilities[i] is KeywordLine handled && handled.Keywords.All(k => MTGEngine.Core.Keywords.Handles(k.Keyword)))
                return abilities.Take(i + 1);
        // "When this creature enters, it connives. (draw a card, then discard a card. …)": the reminder of a keyword
        // action the engine carries out, at the end of the line, explains the action.
        if (line.Text.TrimEnd().EndsWith(')'))
            for (var i = 0; i < abilities.Count - 1; i++)
                if (MTGRulesIR.Lowering.NodeWalker.Nodes(abilities[i]).OfType<KeywordAction>().Any(x => x.Actions.All(a => KeywordActions.Supported.Contains(a.Keyword))))
                    return abilities.Take(i + 1);
        return keywordAt < 0 ? abilities : abilities.Where((ability, i) => i <= keywordAt || ability is not SpellAbility);
    }

    /// <summary>An ability word or flavor word only labels the ability it introduces; the engine acts on the ability.</summary>
    public static Ability Unwrap(Ability ability) =>
        ability switch
        {
            LabeledAbility labeled => Unwrap(labeled.Ability),
            FlavorAbility flavor => Unwrap(flavor.Ability),
            // "You may play lands from the top of your library", "you may cast this card from your graveyard as long as
            // you control a zombie": a permission that holds while its card is where it works (a static, read by Game).
            EffectLine { Effect: Optional { Effect: MayPlayFrom optional } } => PlayFromStatic(optional),
            EffectLine { Effect: MayPlayFrom permission } => PlayFromStatic(permission),
            _ when OfferedAlternative(ability) is { } offered => offered,
            _ => ability,
        };

    /// <summary>"If you attacked this turn, you may pay {u} rather than pay this spell's mana cost": the alternative cost, under its condition.</summary>
    public static AlternativeCost OfferedAlternative(Ability ability)
    {
        var effect = ability switch
        {
            SpellAbility spell => spell.Effect,
            EffectLine line => line.Effect,
            _ => null,
        };
        return effect switch
        {
            AlternativeCostOffer { Alternative: { Replaces: "ThisSpellsManaCost" } alternative } => alternative,
            Conditional { If: var holds, Then: AlternativeCostOffer { Alternative: { Replaces: "ThisSpellsManaCost", If: null } alternative }, Else: null } => alternative with { If = holds },
            _ => null,
        };
    }

    /// <summary>A permanent's effect line that states a continuous effect, as the static ability it is: "this creature attacks each combat if able", "as long as …, it gets +1/+1".</summary>
    public static Ability AsStatic(Ability ability) =>
        ability switch
        {
            EffectLine { Effect: var effect } when Characteristics.IsContinuous(effect) => new StaticAbility(effect),
            // "You may play an additional land on each of your turns": the "may" is the permission itself.
            EffectLine { Effect: Optional { Effect: ExtraLandPlays extra } } when Characteristics.IsContinuous(extra) => new StaticAbility(extra),
            EffectLine { Effect: Conditional { If: var holds, Then: var then, Else: null } } when then is not null && Characteristics.IsContinuous(then) => new StaticAbility(then, holds),
            // "Enchanted creature gets -x/-x, where x is …": a pump of -1/-1 for each of what x counts.
            EffectLine { Effect: LetVariable { Effect: Pump { Duration: null, PerEach: null } pump, Definition: var x } } when PerX(pump.Power) is { } power && PerX(pump.Toughness) is { } toughness
                => new StaticAbility(pump with { Power = new Fixed(power), Toughness = new Fixed(toughness), PerEach = x }),
            _ => ability,
        };

    /// <summary>A pump amount as a multiple of x: x is 1, -x is -1, 0 is 0; null for any other.</summary>
    static int? PerX(Quantity amount) =>
        amount switch
        {
            null or Fixed { Value: 0 } => 0,
            Variable { Name: "X" or "x" } => 1,
            Negated { Of: Variable { Name: "X" or "x" } } => -1,
            _ => null,
        };

    static StaticAbility PlayFromStatic(MayPlayFrom permission) =>
        new(permission with { Duration = null }, permission.Duration is While holds ? holds.Condition : null);

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
    public bool IsSaga => Subtypes.Contains("Saga");

    /// <summary>A saga's last chapter number; null when it has no chapters.</summary>
    public int? FinalChapter => Abilities.OfType<ChapterAbility>().SelectMany(x => x.Chapters).DefaultIfEmpty().Max() is var last and > 0 ? last : null;
    public bool IsPermanentCard => IsCreature || IsLand || IsArtifact || IsEnchantment || IsPlaneswalker || Types.Contains("Battle");
    public bool IsBasicLand => IsLand && Supertypes.Contains("Basic");

    /// <summary>The spell's effect(s) when it's an instant or sorcery: every spell ability's effect, in order.</summary>
    public IEnumerable<Effect> SpellEffects => Abilities.OfType<SpellAbility>().Select(x => x.Effect);

    public override string ToString() => Name;
}
