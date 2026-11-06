using System.Collections;

namespace MTGRulesIR.Lowering;

/// <summary>
/// Repairs "it" and "them" bound to the card itself where they mean cards the effect just found: "look at the top
/// four cards of your library. you may reveal a human card from among them and put it into your hand" on a creature
/// had "from among them" and "put it" both meaning this creature, which would put the creature into its owner's hand.
/// After a step that reveals, looks at or searches for other cards, moving "this" means moving those cards, and
/// "from among this" means from among them; so does a later "it" bound outside the steps ("whenever this creature or
/// another elemental you control enters, look at the top card of your library. if it's a land card, you may put it
/// onto the battlefield tapped" had "it" as the elemental).
/// </summary>
public static class SelfReferenceRepair
{
    static readonly ObjectSelector _thoseCards = new(Quantifier.Those, new ObjectFilter(Kinds: [new Term(nameof(ObjectKind.Card), nameof(ObjectKind))]));

    /// <summary>What the step before made: the token created, the creature manifested.</summary>
    static readonly ObjectSelector _thoseMade = new(Quantifier.Those, new ObjectFilter());

    public static Ability Repair(Ability ability) => (Ability)Rewrite(ability, Fix);

    /// <summary>
    /// A line's spell abilities read as one sequence of steps: "reveal the top card of target opponent's library. if
    /// it's a land, you gain 1 life" lowers as two spell abilities, and "it's" is the card revealed by the first.
    /// </summary>
    public static List<Ability> RepairLine(List<Ability> abilities)
    {
        var spells = abilities.Select((ability, i) => (ability, i)).Where(x => x.ability is SpellAbility).ToList();
        if (spells.Count < 2)
            return abilities;
        var steps = spells.Select(x => ((SpellAbility)x.ability).Effect).ToList();
        if (Repaired(steps) is not { } repaired)
            return abilities;
        var result = abilities.ToList();
        for (var n = 0; n < spells.Count; n++)
            result[spells[n].i] = ((SpellAbility)spells[n].ability) with { Effect = repaired[n] };
        return result;
    }

    static IrNode Fix(IrNode node) =>
        node switch
        {
            Related { Relation: Relation.Among, To: Self } among => among with { To = _thoseCards },
            // "You may have target creature block it this turn if able": "it" is this creature, not the blocker.
            MustBlock { Blockers: ObjectSelector { Binding: { } blockers }, Attacker: Reference { Binding: var it } } must when it == blockers => must with { Attacker = new Self() },
            Sequence sequence when Repaired(sequence.Steps) is { } steps => new Sequence(steps),
            // "Destroy target creature if its toughness is 2 or less": "its" is the target the effect names.
            Conditional { If: { } condition, Then: { } then } conditional when TargetOf(then) is { } target && Unbound(condition) =>
                conditional with { If = (Condition)Rewrite(condition, x => x is CharacteristicOf { Of: Reference { Binding: null, Text: "its" or "it" or "it's" } it } of ? of with { Of = it with { Binding = target } } : x) },
            _ => node,
        };

    static List<Effect> Repaired(IReadOnlyList<Effect> steps)
    {
        List<Effect> repaired = null;
        var local = steps.SelectMany(NodeWalker.Nodes).OfType<ObjectSelector>().Select(x => x.Binding).Where(x => x is not null).ToHashSet();
        var found = false;
        var made = false;
        for (var i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            // "Manifest dread, then attach this equipment to that creature", "create a 1/1 token, then attach this to it".
            if (made)
                step = (Effect)Rewrite(step, node => node is Reference { Text: "it" or "that creature" or "that token" } that && that.Binding is null ? _thoseMade : node);
            if (i > 0 && step is ChangeZone { Objects: Self } move && FindsOtherCards(steps[i - 1]))
                step = move with { Objects = _thoseCards };
            // "Tap that creature and it doesn't untap …", "destroy that creature. it can't be regenerated": "it" is what was just tapped or destroyed.
            if (i > 0 && step is DoesntUntap { Objects: Self } held && steps[i - 1] is Tap { Objects: not (Self or null) } tapped)
                step = held with { Objects = tapped.Objects };
            if (i > 0 && step is CantBeRegenerated { Objects: Self } doomed && steps[i - 1] is Destroy { Objects: not (Self or null) } destroyed)
                step = doomed with { Objects = destroyed.Objects };
            // "An opponent chooses a permanent you control other than this creature and exiles it", "choose target permanent card in your graveyard … return it to your hand": "it" is what was chosen.
            if (i > 0 && steps[i - 1] is ChooseEffect { From: [ObjectSelector { Binding: { } chosen }] } && WithObjects(step, new Reference(chosen, "it")) is { } acting)
                step = acting;
            // "You draw cards equal to the sacrificed creature's power, then you gain life equal to its toughness": "its" is that creature.
            if (i > 0 && NodeWalker.Nodes(steps[i - 1]).OfType<CharacteristicOf>().Select(x => x.Of).OfType<ObjectSelector>().FirstOrDefault(x => x.Quantifier == Quantifier.Those) is { } measured)
                step = (Effect)Rewrite(step, node => node is CharacteristicOf { Of: Reference { Binding: null, Text: "its" or "it" } } of ? of with { Of = measured } : node);
            if (found)
                step = (Effect)Rewrite(step, node => node is Reference { Text: "it" or "it's" } it && (it.Binding is null || !local.Contains(it.Binding)) ? _thoseCards : node);
            if (!ReferenceEquals(step, steps[i]))
            {
                repaired ??= [.. steps];
                repaired[i] = step;
            }
            found |= FindsOtherCards(steps[i]);
            made = steps[i] is CreateTokens || steps[i] is KeywordAction { Actions: [{ Keyword: "Manifest" or "ManifestDread" or "Amass" }, ..] } || made && steps[i] is not (Reveal or LookAt or SearchLibrary);
        }
        return repaired;
    }

    /// <summary>The binding of the one target an effect names; null when it names none or several.</summary>
    static string TargetOf(Effect effect)
    {
        var bindings = NodeWalker.Nodes(effect).OfType<ObjectSelector>().Where(x => x.Quantifier == Quantifier.Target).Select(x => x.Binding).Distinct().ToList();
        return bindings is [{ } only] ? only : null;
    }

    /// <summary>Whether the condition reads a characteristic of an unbound "its" ("if its toughness is 2 or less").</summary>
    static bool Unbound(Condition condition) =>
        NodeWalker.Nodes(condition).OfType<CharacteristicOf>().Any(x => x.Of is Reference { Binding: null, Text: "its" or "it" or "it's" });

    /// <summary><paramref name="step"/> acting on <paramref name="objects"/> where it acted on this object; null when it doesn't.</summary>
    static Effect WithObjects(Effect step, Selector objects)
    {
        var property = step.GetType().GetProperty("Objects");
        if (property is null || !property.CanWrite || property.PropertyType != typeof(Selector) || property.GetValue(step) is not Self)
            return null;
        var copy = (Effect)step.GetType().GetMethod("<Clone>$")!.Invoke(step, null);
        property.SetValue(copy, objects);
        return copy;
    }

    static bool FindsOtherCards(Effect step) =>
        step switch
        {
            Reveal { What: not (Self or null) } => true,
            LookAt { What: not (Self or null) } => true,
            SearchLibrary => true,
            _ => false,
        };

    /// <summary>The tree with <paramref name="fix"/> applied bottom-up; nodes whose children didn't change are kept as they are.</summary>
    public static IrNode Rewrite(IrNode node, Func<IrNode, IrNode> fix)
    {
        if (node is null)
            return null;
        IrNode copy = null;
        foreach (var property in NodeWalker.PropertiesOf(node.GetType()))
        {
            if (!property.CanWrite)
                continue;
            var value = property.GetValue(node);
            object replaced = value switch
            {
                IrNode child => Rewrite(child, fix) is var rewritten && !ReferenceEquals(rewritten, child) ? rewritten : null,
                IEnumerable list and not string when property.PropertyType.IsGenericType && typeof(IrNode).IsAssignableFrom(property.PropertyType.GetGenericArguments()[0]) => RewriteList(list, property.PropertyType.GetGenericArguments()[0], fix),
                _ => null,
            };
            if (replaced is null)
                continue;
            copy ??= (IrNode)node.GetType().GetMethod("<Clone>$")!.Invoke(node, null);
            property.SetValue(copy, replaced);
        }
        return fix(copy ?? node);
    }

    static object RewriteList(IEnumerable list, Type element, Func<IrNode, IrNode> fix)
    {
        var items = list.Cast<object>().ToList();
        var rewritten = items.Select(x => x is IrNode child ? Rewrite(child, fix) : x).ToList();
        if (rewritten.Zip(items).All(x => ReferenceEquals(x.First, x.Second)))
            return null;
        var typed = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(element))!;
        foreach (var item in rewritten)
            typed.Add(item);
        return typed;
    }
}
