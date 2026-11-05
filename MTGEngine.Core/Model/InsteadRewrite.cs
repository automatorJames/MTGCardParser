using System.Reflection;

namespace MTGEngine.Core;

/// <summary>
/// "Deal 2 damage to any target. If this spell was kicked, it deals 4 damage instead." The second sentence replaces
/// the first rather than adding to it: a step followed by "if C, instead E" becomes "if C then E else the step", with
/// what E leaves unsaid (its target, its source) taken from the step it replaces.
/// </summary>
public static class InsteadRewrite
{
    public static IEnumerable<Ability> Rewrite(IEnumerable<Ability> abilities)
    {
        var list = abilities.Select(RewriteAbility).ToList();
        for (var i = 1; i < list.Count; i++)
            if (list[i] is SpellAbility { Effect: StillOfKind { Kinds: [var kind, ..] } } && list[i - 1] is SpellAbility before)
            {
                list[i - 1] = new SpellAbility(Still(before.Effect, kind));
                list.RemoveAt(i);
                i--;
            }
        for (var i = 1; i < list.Count; i++)
            if (list[i] is SpellAbility { Effect: var effect } && IsInstead(effect) && list[i - 1] is SpellAbility previous)
            {
                list[i - 1] = new SpellAbility(Merge(previous.Effect, (Conditional)effect));
                list.RemoveAt(i);
                i--;
            }
        return list;
    }

    static Ability RewriteAbility(Ability ability) =>
        ability switch
        {
            SpellAbility spell => spell with { Effect = RewriteEffect(spell.Effect) },
            TriggeredAbility triggered => triggered with { Effect = RewriteEffect(triggered.Effect) },
            ActivatedAbility activated => activated with { Effect = RewriteEffect(activated.Effect) },
            LoyaltyAbility loyalty => loyalty with { Effect = RewriteEffect(loyalty.Effect) },
            ChapterAbility chapter => chapter with { Effect = RewriteEffect(chapter.Effect) },
            _ => ability,
        };

    /// <summary>"Becomes a 3/3 elemental creature. It's still a land.": the animation keeps the kind.</summary>
    static Effect Still(Effect effect, string kind) =>
        effect switch
        {
            Animate animate => animate with { Still = animate.Still ?? kind },
            Lasting lasting => lasting with { Effect = Still(lasting.Effect, kind) },
            Optional optional => optional with { Effect = Still(optional.Effect, kind) },
            Conditional conditional => conditional with { Then = Still(conditional.Then, kind) },
            Sequence sequence when sequence.Steps.Count > 0 => new Sequence([.. sequence.Steps.Take(sequence.Steps.Count - 1), Still(sequence.Steps[^1], kind)]),
            _ => effect,
        };

    static Effect RewriteEffect(Effect effect)
    {
        if (effect is not Sequence sequence)
            return effect;
        var steps = sequence.Steps.Select(RewriteEffect).ToList();
        for (var i = 1; i < steps.Count; i++)
            if (steps[i] is StillOfKind { Kinds: [var kind, ..] })
            {
                steps[i - 1] = Still(steps[i - 1], kind);
                steps.RemoveAt(i);
                i--;
            }
        for (var i = 1; i < steps.Count; i++)
            if (IsInstead(steps[i]) && !IsInstead(steps[i - 1]))
            {
                steps[i - 1] = Merge(steps[i - 1], (Conditional)steps[i]);
                steps.RemoveAt(i);
                i--;
            }
        return new Sequence(steps);
    }

    static bool IsInstead(Effect effect) => effect is Conditional { Then: Instead, Else: null };

    static Effect Merge(Effect previous, Conditional conditional) =>
        new Conditional(conditional.If, Fill(((Instead)conditional.Then).Effect, previous), previous);

    /// <summary><paramref name="replacement"/> with the parts it leaves out (null properties, damage without a recipient) taken from <paramref name="previous"/> when they're the same kind of effect.</summary>
    public static Effect Fill(Effect replacement, Effect previous)
    {
        if (replacement.GetType() != previous.GetType())
            return replacement;

        if (replacement is DealDamage damage && previous is DealDamage before)
            return damage with
            {
                Source = damage.Source ?? before.Source,
                Portions = damage.Portions.Select((portion, i) => portion.To is null && before.Portions.Count > 0 ? portion with { To = before.Portions[Math.Min(i, before.Portions.Count - 1)].To } : portion).ToList(),
            };

        var clone = (Effect)replacement.GetType().GetMethod("<Clone>$")!.Invoke(replacement, null);
        foreach (var property in replacement.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            if (property.CanWrite && property.GetValue(replacement) is null && property.GetValue(previous) is { } value)
                property.SetValue(clone, value);
        return clone;
    }
}
