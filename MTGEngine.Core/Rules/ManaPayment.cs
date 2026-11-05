namespace MTGEngine.Core;

/// <summary>
/// Paying mana costs from the pool and from untapped mana sources: basic lands by their type, and permanents with a
/// "{T}: add …" ability. Finds an assignment of sources to the cost's symbols (colored first, then hybrid, then
/// generic) and taps them. Mana abilities don't use the stack.
/// </summary>
public static class ManaPayment
{
    /// <summary>What a payment is for, so mana that can be spent only on some things is used only for those: the spell being cast, or the object whose ability is being activated.</summary>
    public sealed record Purpose(GameObject Spell = null, GameObject Activating = null);

    /// <summary>Whether a mana restriction ("spend this mana only to cast creature spells", "can't be spent to cast nonartifact spells") lets its mana pay for <paramref name="purpose"/>.</summary>
    public static bool Allows(ManaRestriction restriction, Purpose purpose, GameState state, GameObject source) =>
        Allows(restriction, purpose, state, source.Controller, source.Id);

    public static bool Allows(ManaRestriction restriction, Purpose purpose, GameState state, PlayerId controller, ObjectId sourceId)
    {
        bool Use(Playable use) => use.Verb switch
        {
            "Cast" => purpose?.Spell is { } spell && (use.What is not ObjectSelector { Filter: var filter } || Filters.Matches(state, filter with { Zone = null }, spell with { Zone = Zone.Stack }, controller, sourceId)),
            "Activate" => purpose?.Activating is not null,
            _ => false,
        };
        return restriction.Rule == "NotFor" ? !restriction.Uses.Any(Use) : restriction.Uses.Any(Use);
    }

    /// <summary>The pool's mana that can pay for <paramref name="purpose"/>: the unrestricted, and restricted mana whose restriction allows it.</summary>
    public static ManaPool Usable(GameState state, PlayerId player, Purpose purpose)
    {
        var you = state.Player(player);
        var pool = you.Pool;
        foreach (var mana in you.RestrictedList)
            if (Allows(mana.Restriction, purpose, state, mana.Controller, mana.Source))
                pool = mana.Color is { } color ? pool.Add(color, mana.Amount) : pool.AddColorless(mana.Amount);
        return pool;
    }

    /// <summary>A permanent that can be tapped for mana now, with the colors it could make (null entry = colorless).</summary>
    public sealed record Source(ObjectId Id, ImmutableArray<Color?> Options, int Amount = 1, bool Snow = false, bool Delve = false);

    public static ImmutableList<Source> Sources(GameState state, PlayerId player, bool convoke = false, Purpose purpose = null)
    {
        List<Source> sources = [];

        foreach (var obj in state.Battlefield.Where(x => x.Controller == player && !x.Tapped))
        {
            var traits = Characteristics.Of(state, obj);
            if (traits.IsCreature && obj.EnteredThisTurn && !traits.HasKeyword("Haste"))
                continue;
            if (traits.Cant("Activate"))
                continue;

            List<Color?> options = [];
            var amount = 1;

            foreach (var subtype in traits.Subtypes)
                if (Colors.OfBasicLandType(subtype) is { } color)
                    options.Add(color);

            for (var index = 0; index < traits.Abilities.Count; index++)
            {
                if (traits.Abilities[index] is not ActivatedAbility ability)
                    continue;
                if (!ability.Costs.All(x => x is TapSelf))
                    continue;
                // "Add x mana in any combination of colors, where x is …": the mana, read as one.
                if ((ability.Effect is LetVariable { Effect: AddMana inner } ? inner : ability.Effect) is not AddMana add)
                    continue;
                // "Spend this mana only to …" follows the mana ability it restricts.
                if (index + 1 < traits.Abilities.Count && traits.Abilities[index + 1] is StaticAbility { Effect: ManaRestriction restriction } && !Allows(restriction, purpose, state, obj))
                    continue;
                foreach (var option in add.Options)
                    switch (option)
                    {
                        case ManaAmount { Count: null } mana:
                        {
                            var colors = mana.Symbols.Select(SymbolColor).ToList();
                            if (colors.All(x => x.Known))
                            {
                                amount = Math.Max(amount, colors.Count);
                                foreach (var c in colors.Where(c => colors.Count == 1))
                                    options.Add(c.Color);
                                if (colors.Count > 1 && colors.Select(x => x.Color).Distinct().Count() == 1)
                                    options.Add(colors[0].Color);
                            }
                            break;
                        }
                        case ManaChoice { Choice: "AnyColor" or "AnyOneColor" or "AnyCombination" }:
                            options.AddRange(Colors.All.Select(x => (Color?)x));
                            break;
                        case ManaChoice { Choice: "TheChosenColor" } when Colors.OfName(obj.Chosen("Color")) is { } chosen:
                            options.Add(chosen);
                            break;
                    }
            }

            if (options.Count > 0)
                sources.Add(new Source(obj.Id, options.Distinct().ToImmutableArray(), amount, obj.Card.Supertypes.Contains("Snow")));
        }

        // Convoke: each untapped creature (summoning sick or not) can pay for {1} or one mana of its color.
        if (convoke)
            foreach (var creature in state.Battlefield.Where(x => x.Controller == player && !x.Tapped && sources.All(s => s.Id != x.Id)))
                if (Characteristics.Of(state, creature) is { IsCreature: true } traits)
                    sources.Add(new Source(creature.Id, [.. traits.Colors.Select(c => (Color?)c), null]));

        // Improvise: each untapped artifact can pay for {1}. Delve: each card exiled from your graveyard pays for {1}.
        if (purpose?.Spell is { } spell)
        {
            if (spell.Card.Keywords.Contains("Improvise"))
                foreach (var artifact in state.Battlefield.Where(x => x.Controller == player && !x.Tapped && sources.All(s => s.Id != x.Id)))
                    if (Characteristics.Of(state, artifact).Types.Contains("Artifact"))
                        sources.Add(new Source(artifact.Id, [null]));
            if (spell.Card.Keywords.Contains("Delve"))
                foreach (var card in state.Player(player).Graveyard.Where(x => x != spell.Id))
                    sources.Add(new Source(card, [null], Delve: true));
        }

        return sources.ToImmutableList();
    }

    static (bool Known, Color? Color) SymbolColor(string symbol)
    {
        var inner = symbol.Trim('{', '}');
        if (inner.Equals("C", StringComparison.OrdinalIgnoreCase))
            return (true, null);
        var color = Colors.OfSymbol(inner);
        return (color is not null, color);
    }

    /// <summary>The sources to tap, and the color each makes, to pay <paramref name="cost"/> together with the pool; null when it can't be paid.</summary>
    public static ImmutableList<(Source Source, Color? Color)> Plan(GameState state, PlayerId player, ManaCost cost, bool convoke = false, Purpose purpose = null)
    {
        var pool = Usable(state, player, purpose);
        if (pool.Pay(cost) is not null)
            return [];

        var sources = Sources(state, player, convoke, purpose);
        // Requirements: colored symbols, hybrid symbols, then generic (any color or colorless) and colorless-only.
        List<ImmutableArray<Color?>> requirements = [];
        foreach (var (color, amount) in cost.Colored)
            for (var i = 0; i < amount; i++)
                requirements.Add([color]);
        foreach (var option in cost.Hybrid)
            requirements.Add(option.Select(x => (Color?)x).ToImmutableArray());
        for (var i = 0; i < cost.Colorless; i++)
            requirements.Add([null]);
        for (var i = 0; i < cost.Generic; i++)
            requirements.Add([..Colors.All.Select(x => (Color?)x), null]);

        // Use the pool first: remove requirements the pool can satisfy.
        var poolColored = pool.Colored.ToBuilder();
        var poolColorless = pool.Colorless;
        List<ImmutableArray<Color?>> remaining = [];
        foreach (var requirement in requirements.OrderBy(x => x.Length))
        {
            var paid = false;
            foreach (var option in requirement)
            {
                if (option is { } color && poolColored.GetValueOrDefault(color) > 0)
                {
                    poolColored[color] = poolColored[color] - 1;
                    paid = true;
                    break;
                }
                if (option is null && poolColorless > 0)
                {
                    poolColorless--;
                    paid = true;
                    break;
                }
            }
            if (!paid)
                remaining.Add(requirement);
        }

        if (remaining.Count == 0 && cost.Snow == 0)
            return [];

        // Snow symbols need snow sources, so the others are paid from non-snow sources where they can be.
        sources = sources.OrderBy(x => x.Snow).ThenBy(x => x.Delve).ToImmutableList();
        var used = new bool[sources.Count];
        var plan = new List<(Source, Color?)>();
        if (!Assign(remaining.OrderBy(x => x.Length).ToList(), 0, sources, used, plan))
            return null;
        for (var snow = 0; snow < cost.Snow; snow++)
        {
            var index = Enumerable.Range(0, sources.Count).FirstOrDefault(i => !used[i] && sources[i].Snow, -1);
            if (index < 0)
                return null;
            used[index] = true;
            plan.Add((sources[index], sources[index].Options[0]));
        }
        return plan.ToImmutableList();
    }

    static bool Assign(List<ImmutableArray<Color?>> requirements, int index, ImmutableList<Source> sources, bool[] used, List<(Source, Color?)> plan)
    {
        if (index == requirements.Count)
            return true;

        var requirement = requirements[index];
        for (var i = 0; i < sources.Count; i++)
        {
            if (used[i])
                continue;
            foreach (var option in sources[i].Options)
            {
                if (!requirement.Contains(option))
                    continue;
                used[i] = true;
                plan.Add((sources[i], option));
                if (Assign(requirements, index + 1, sources, used, plan))
                    return true;
                plan.RemoveAt(plan.Count - 1);
                used[i] = false;
                break;
            }
        }
        return false;
    }

    public static bool CanPay(GameState state, PlayerId player, ManaCost cost, bool convoke = false, Purpose purpose = null) => Plan(state, player, cost, convoke, purpose) is not null;

    /// <summary>The largest X for which <paramref name="cost"/> is payable.</summary>
    public static int MaxX(GameState state, PlayerId player, ManaCost cost, Purpose purpose = null)
    {
        var x = 0;
        while (x < 30 && CanPay(state, player, cost.WithX(x + 1), purpose: purpose))
            x++;
        return x;
    }
}
