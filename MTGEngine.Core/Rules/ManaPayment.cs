namespace MTGEngine.Core;

/// <summary>
/// Paying mana costs from the pool and from untapped mana sources: basic lands by their type, and permanents with a
/// "{T}: add …" ability. Finds an assignment of sources to the cost's symbols (colored first, then hybrid, then
/// generic) and taps them. Mana abilities don't use the stack.
/// </summary>
public static class ManaPayment
{
    /// <summary>A permanent that can be tapped for mana now, with the colors it could make (null entry = colorless).</summary>
    public sealed record Source(ObjectId Id, ImmutableArray<Color?> Options, int Amount = 1);

    public static ImmutableList<Source> Sources(GameState state, PlayerId player)
    {
        List<Source> sources = [];

        foreach (var obj in state.Battlefield.Where(x => x.Controller == player && !x.Tapped))
        {
            var traits = Characteristics.Of(state, obj);
            if (traits.IsCreature && obj.EnteredThisTurn && !traits.HasKeyword("Haste"))
                continue;

            List<Color?> options = [];
            var amount = 1;

            foreach (var subtype in traits.Subtypes)
                if (Colors.OfBasicLandType(subtype) is { } color)
                    options.Add(color);

            foreach (var ability in traits.Abilities.OfType<ActivatedAbility>())
            {
                if (!ability.Costs.All(x => x is TapSelf))
                    continue;
                if (ability.Effect is not AddMana add)
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
                        case ManaChoice { Choice: "AnyColor" or "AnyOneColor" }:
                            options.AddRange(Colors.All.Select(x => (Color?)x));
                            break;
                    }
            }

            if (options.Count > 0)
                sources.Add(new Source(obj.Id, options.Distinct().ToImmutableArray(), amount));
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
    public static ImmutableList<(Source Source, Color? Color)> Plan(GameState state, PlayerId player, ManaCost cost)
    {
        var pool = state.Player(player).Pool;
        if (pool.Pay(cost) is not null)
            return [];

        var sources = Sources(state, player);
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

        if (remaining.Count == 0)
            return [];

        var used = new bool[sources.Count];
        var plan = new List<(Source, Color?)>();
        return Assign(remaining.OrderBy(x => x.Length).ToList(), 0, sources, used, plan) ? plan.ToImmutableList() : null;
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

    public static bool CanPay(GameState state, PlayerId player, ManaCost cost) => Plan(state, player, cost) is not null;

    /// <summary>The largest X for which <paramref name="cost"/> is payable.</summary>
    public static int MaxX(GameState state, PlayerId player, ManaCost cost)
    {
        var x = 0;
        while (x < 30 && CanPay(state, player, cost.WithX(x + 1)))
            x++;
        return x;
    }
}
