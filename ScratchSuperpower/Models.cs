using System.Text.Json.Serialization;

namespace MTGCardParser;

// --- Add derived type hints to all interfaces for proper serialization ---

[JsonDerivedType(typeof(StaticAbility), typeDiscriminator: "Static")]
[JsonDerivedType(typeof(ActivatedAbility), typeDiscriminator: "Activated")]
[JsonDerivedType(typeof(TriggeredAbility), typeDiscriminator: "Triggered")]
public interface IAbility { }

[JsonDerivedType(typeof(TapCost), typeDiscriminator: "Tap")]
[JsonDerivedType(typeof(ManaCost), typeDiscriminator: "Mana")]
public interface ICost { }

[JsonDerivedType(typeof(AddManaEffect), typeDiscriminator: "AddMana")]
[JsonDerivedType(typeof(GainLifeEffect), typeDiscriminator: "GainLife")]
[JsonDerivedType(typeof(DealDamageEffect), typeDiscriminator: "DealDamage")]
[JsonDerivedType(typeof(OptionalEffect), typeDiscriminator: "Optional")]
public interface IEffect { }

public interface ITarget { }

// --- Concrete Record types remain the same ---

public record StaticAbility(string Keyword) : IAbility;
public record ActivatedAbility(List<ICost> Costs, List<IEffect> Effects) : IAbility;
public record TriggeredAbility(string TriggerCondition, List<IEffect> Effects) : IAbility;

public record TapCost : ICost;
public record ManaCost(int Generic = 0, int W = 0, int U = 0, int B = 0, int R = 0, int G = 0) : ICost
{
    public override string ToString()
    {
        var parts = new List<string>();
        if (Generic > 0) parts.Add($"{{{Generic}}}");
        for (int i = 0; i < W; i++) parts.Add("{W}");
        for (int i = 0; i < U; i++) parts.Add("{U}");
        for (int i = 0; i < B; i++) parts.Add("{B}");
        for (int i = 0; i < R; i++) parts.Add("{R}");
        for (int i = 0; i < G; i++) parts.Add("{G}");
        return string.Concat(parts);
    }
}

public record AddManaEffect(ManaCost Mana) : IEffect;
public record GainLifeEffect(int Amount) : IEffect;
public record DealDamageEffect(int Amount, ITarget Target) : IEffect;
public record OptionalEffect(IEffect Effect) : IEffect;

public record AnyTarget : ITarget
{
    public override string ToString() => "any target";
}