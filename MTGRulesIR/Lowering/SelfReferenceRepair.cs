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

    public static Ability Repair(Ability ability) => (Ability)Rewrite(ability, Fix);

    static IrNode Fix(IrNode node) =>
        node switch
        {
            Related { Relation: Relation.Among, To: Self } among => among with { To = _thoseCards },
            Sequence sequence when Repaired(sequence.Steps) is { } steps => new Sequence(steps),
            _ => node,
        };

    static List<Effect> Repaired(IReadOnlyList<Effect> steps)
    {
        List<Effect> repaired = null;
        var local = steps.SelectMany(NodeWalker.Nodes).OfType<ObjectSelector>().Select(x => x.Binding).Where(x => x is not null).ToHashSet();
        var found = false;
        for (var i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            if (i > 0 && step is ChangeZone { Objects: Self } move && FindsOtherCards(steps[i - 1]))
                step = move with { Objects = _thoseCards };
            // "Tap that creature and it doesn't untap …", "destroy that creature. it can't be regenerated": "it" is what was just tapped or destroyed.
            if (i > 0 && step is DoesntUntap { Objects: Self } held && steps[i - 1] is Tap { Objects: not (Self or null) } tapped)
                step = held with { Objects = tapped.Objects };
            if (i > 0 && step is CantBeRegenerated { Objects: Self } doomed && steps[i - 1] is Destroy { Objects: not (Self or null) } destroyed)
                step = doomed with { Objects = destroyed.Objects };
            if (found)
                step = (Effect)Rewrite(step, node => node is Reference { Text: "it" or "it's" } it && (it.Binding is null || !local.Contains(it.Binding)) ? _thoseCards : node);
            if (!ReferenceEquals(step, steps[i]))
            {
                repaired ??= [.. steps];
                repaired[i] = step;
            }
            found |= FindsOtherCards(steps[i]);
        }
        return repaired;
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
    static IrNode Rewrite(IrNode node, Func<IrNode, IrNode> fix)
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
