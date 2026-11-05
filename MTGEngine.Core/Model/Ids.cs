using System.Text.RegularExpressions;

namespace MTGEngine.Core;

/// <summary>Identifies one game object - a card in any zone, a token, a copy - for the whole game, across zone changes.</summary>
public readonly record struct ObjectId(int Value)
{
    public override string ToString() => $"#{Value}";
}

public readonly record struct PlayerId(int Value)
{
    public override string ToString() => $"P{Value}";
}

/// <summary>An object or a player, as the thing a spell targets, damage goes to, or a reference points at.</summary>
public readonly record struct TargetRef(ObjectId? Object, PlayerId? Player)
{
    public static TargetRef Of(ObjectId id) => new(id, null);
    public static TargetRef Of(PlayerId id) => new(null, id);
    public bool IsObject => Object is not null;
    public bool IsPlayer => Player is not null;
    public override string ToString() => Object?.ToString() ?? Player?.ToString() ?? "-";
}

public enum Zone { Library, Hand, Battlefield, Graveyard, Stack, Exile, Command }

public enum Step { Untap, Upkeep, Draw, Main1, BeginCombat, DeclareAttackers, DeclareBlockers, CombatDamage, EndCombat, Main2, End, Cleanup }

public enum Color { White, Blue, Black, Red, Green }

public static class Colors
{
    public static readonly Color[] All = [Color.White, Color.Blue, Color.Black, Color.Red, Color.Green];

    public static Color? OfSymbol(string symbol) =>
        symbol.ToUpperInvariant() switch
        {
            "W" => Color.White,
            "U" => Color.Blue,
            "B" => Color.Black,
            "R" => Color.Red,
            "G" => Color.Green,
            _ => null,
        };

    public static Color? OfName(string name) =>
        name?.ToLowerInvariant() switch
        {
            "white" => Color.White,
            "blue" => Color.Blue,
            "black" => Color.Black,
            "red" => Color.Red,
            "green" => Color.Green,
            _ => null,
        };

    public static Color? OfBasicLandType(string subtype) =>
        subtype switch
        {
            "Plains" => Color.White,
            "Island" => Color.Blue,
            "Swamp" => Color.Black,
            "Mountain" => Color.Red,
            "Forest" => Color.Green,
            _ => null,
        };
}

/// <summary>
/// A mana cost: generic mana, colored mana by color, hybrid symbols (any one of their colors), X symbols, and
/// colorless-only mana ({C}). Phyrexian symbols count as their color here; snow and other riders are ignored.
/// </summary>
public sealed record ManaCost(int Generic, ImmutableDictionary<Color, int> Colored, ImmutableList<ImmutableArray<Color>> Hybrid, int X = 0, int Colorless = 0)
{
    public static readonly ManaCost Free = new(0, ImmutableDictionary<Color, int>.Empty, ImmutableList<ImmutableArray<Color>>.Empty);

    static readonly Regex _symbol = new(@"\{([^}]+)\}");

    public static ManaCost Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Free;

        int generic = 0, x = 0, colorless = 0;
        var colored = ImmutableDictionary.CreateBuilder<Color, int>();
        var hybrid = ImmutableList.CreateBuilder<ImmutableArray<Color>>();

        foreach (Match match in _symbol.Matches(text))
        {
            var symbol = match.Groups[1].Value.ToUpperInvariant();

            if (int.TryParse(symbol, out var amount))
                generic += amount;
            else if (symbol == "X")
                x++;
            else if (symbol == "C")
                colorless++;
            else if (symbol.Contains('/'))
            {
                var parts = symbol.Split('/').Select(Colors.OfSymbol).Where(c => c is not null).Select(c => c.Value).ToImmutableArray();
                if (parts.Length == 1)
                    colored[parts[0]] = colored.GetValueOrDefault(parts[0]) + 1;
                else if (parts.Length > 1)
                    hybrid.Add(parts);
                else if (symbol.StartsWith("2/"))
                    generic += 2;
            }
            else if (Colors.OfSymbol(symbol) is { } color)
                colored[color] = colored.GetValueOrDefault(color) + 1;
        }

        return new ManaCost(generic, colored.ToImmutable(), hybrid.ToImmutable(), x, colorless);
    }

    public int ManaValue => Generic + Colorless + Colored.Values.Sum() + Hybrid.Count;

    public int ColoredCount => Colored.Values.Sum() + Hybrid.Count;

    public bool IsFree => ManaValue == 0 && X == 0;

    /// <summary>This cost with each X worth <paramref name="x"/> generic mana.</summary>
    public ManaCost WithX(int x) => this with { Generic = Generic + X * x, X = 0 };

    public override string ToString()
    {
        var parts = new List<string>();
        for (var i = 0; i < X; i++) parts.Add("{X}");
        if (Generic > 0) parts.Add($"{{{Generic}}}");
        for (var i = 0; i < Colorless; i++) parts.Add("{C}");
        foreach (var color in Colors.All)
            for (var i = 0; i < Colored.GetValueOrDefault(color); i++)
                parts.Add("{" + Symbol(color) + "}");
        foreach (var option in Hybrid)
            parts.Add("{" + string.Join("/", option.Select(Symbol)) + "}");
        return parts.Count == 0 ? "{0}" : string.Concat(parts);
    }

    public static string Symbol(Color color) => color switch
    {
        Color.White => "W",
        Color.Blue => "U",
        Color.Black => "B",
        Color.Red => "R",
        _ => "G",
    };
}

/// <summary>Mana a player has available: by color, plus colorless.</summary>
public sealed record ManaPool(ImmutableDictionary<Color, int> Colored, int Colorless = 0)
{
    public static readonly ManaPool Empty = new(ImmutableDictionary<Color, int>.Empty);

    public int Total => Colored.Values.Sum() + Colorless;

    public bool IsEmpty => Total == 0;

    public ManaPool Add(Color color, int amount = 1) =>
        this with { Colored = Colored.SetItem(color, Colored.GetValueOrDefault(color) + amount) };

    public ManaPool AddColorless(int amount = 1) => this with { Colorless = Colorless + amount };

    public int Of(Color color) => Colored.GetValueOrDefault(color);

    /// <summary>The pool after paying <paramref name="cost"/>, or null when it can't: colored first, then hybrid, then generic from whatever's left (colorless first).</summary>
    public ManaPool Pay(ManaCost cost)
    {
        var colored = Colored.ToBuilder();
        var colorless = Colorless;

        foreach (var (color, amount) in cost.Colored)
        {
            if (colored.GetValueOrDefault(color) < amount)
                return null;
            colored[color] = colored[color] - amount;
        }

        foreach (var option in cost.Hybrid)
        {
            var paid = false;
            foreach (var color in option.OrderByDescending(c => colored.GetValueOrDefault(c)))
                if (colored.GetValueOrDefault(color) > 0)
                {
                    colored[color] = colored[color] - 1;
                    paid = true;
                    break;
                }
            if (!paid)
                return null;
        }

        if (colorless < cost.Colorless)
            return null;
        colorless -= cost.Colorless;

        var generic = cost.Generic;
        var fromColorless = Math.Min(generic, colorless);
        colorless -= fromColorless;
        generic -= fromColorless;

        foreach (var color in Colors.All.OrderBy(c => colored.GetValueOrDefault(c)))
        {
            if (generic == 0)
                break;
            var take = Math.Min(generic, colored.GetValueOrDefault(color));
            colored[color] = colored.GetValueOrDefault(color) - take;
            generic -= take;
        }

        return generic == 0 ? new ManaPool(colored.ToImmutable(), colorless) : null;
    }
}
